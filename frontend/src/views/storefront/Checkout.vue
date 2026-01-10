<script setup>
import { computed, ref, watch } from 'vue'
import TopNav from '../../components/ui/TopNav.vue'
import { useCartStore } from '../../stores/cart'
import { useAuthStore } from '../../stores/auth'
import { apiPost } from '../../lib/api'

const cart = useCartStore()
const auth = useAuthStore()

const step = ref(1)
const error = ref(null)
const loading = ref(false)

const address = ref({
  fullName: '',
  line1: '',
  line2: '',
  city: '',
  state: '',
  postalCode: '',
  countryCode: 'IN',
  phone: ''
})

const addressValidation = ref(null)
const splitPreview = ref([])
const paymentSessionId = ref(null)
const couponCode = ref('')

const checkoutItems = computed(() =>
  cart.items.map((i) => ({ productId: i.productId, variantId: i.variantId, quantity: i.quantity }))
)

watch(
  () => cart.items,
  () => {
    if (step.value > 1) step.value = 1
    addressValidation.value = null
    splitPreview.value = []
    paymentSessionId.value = null
  },
  { deep: true }
)

async function validateAddress() {
  error.value = null
  loading.value = true
  try {
    addressValidation.value = await apiPost('/api/storefront/checkout/validate-address', address.value)
    if (!addressValidation.value.isValid) throw new Error(addressValidation.value.message || 'Invalid address')
    step.value = 2
  } catch (e) {
    error.value = e?.message || 'Address validation failed'
  } finally {
    loading.value = false
  }
}

async function loadPreview() {
  error.value = null
  loading.value = true
  try {
    splitPreview.value = await apiPost('/api/storefront/checkout/preview-v2', {
      items: checkoutItems.value,
      couponCode: couponCode.value || null
    })
    step.value = 3
  } catch (e) {
    error.value = e?.message || 'Preview failed'
  } finally {
    loading.value = false
  }
}

async function submitPayment() {
  error.value = null
  loading.value = true
  try {
    if (!auth.isAuthenticated) throw new Error('Please login as a customer to submit checkout.')
    const res = await apiPost('/api/storefront/checkout/submit', {
      shippingAddress: address.value,
      items: checkoutItems.value,
      couponCode: couponCode.value || null
    })
    paymentSessionId.value = res.paymentSessionId
  } catch (e) {
    error.value = e?.message || 'Checkout submit failed'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <TopNav />
  <div class="container">
    <div class="card" style="margin-top:16px;">
      <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
        <div>
          <div class="pill">3-step checkout</div>
          <h2 style="margin:8px 0 0;">Checkout</h2>
        </div>
        <div class="pill">Step {{ step }}/3</div>
      </div>
    </div>

    <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
      <b>Error:</b> {{ error }}
    </div>

    <div class="row" style="align-items:flex-start; margin-top:12px;">
      <div class="col">
        <div class="card">
          <div style="font-weight:900; margin-bottom:10px;">Cart</div>
          <div v-if="!cart.items.length" class="muted">Cart is empty.</div>
          <div v-else style="display:flex; flex-direction:column; gap:10px;">
            <div v-for="i in cart.items" :key="`${i.productId}:${i.variantId}`" class="card" style="background:#f8fafc;">
              <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
                <div>
                  <b>{{ i.name || `Product ${i.productId}` }}</b>
                  <div class="muted" style="font-size:12px;">Variant: {{ i.variantId ?? 'default' }}</div>
                </div>
                <div style="display:flex; gap:8px; align-items:center;">
                  <input class="input" style="width:90px;" type="number" min="1" v-model.number="i.quantity" />
                  <button class="btn danger" @click="cart.removeItem(i.productId, i.variantId)">Remove</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col">
        <div v-if="step === 1" class="card">
          <div style="font-weight:900; margin-bottom:10px;">1) Shipping Address (with validation)</div>
          <div class="row">
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">Full name</div>
              <input class="input" v-model="address.fullName" />
            </label>
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">Phone</div>
              <input class="input" v-model="address.phone" />
            </label>
          </div>
          <label style="display:block; margin-top:10px;">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Address line 1</div>
            <input class="input" v-model="address.line1" />
          </label>
          <label style="display:block; margin-top:10px;">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Address line 2</div>
            <input class="input" v-model="address.line2" />
          </label>
          <div class="row" style="margin-top:10px;">
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">City</div>
              <input class="input" v-model="address.city" />
            </label>
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">State</div>
              <input class="input" v-model="address.state" />
            </label>
          </div>
          <div class="row" style="margin-top:10px;">
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">Postal code</div>
              <input class="input" v-model="address.postalCode" />
            </label>
            <label class="col">
              <div class="muted" style="font-size:12px; margin-bottom:6px;">Country</div>
              <input class="input" v-model="address.countryCode" />
            </label>
          </div>

          <button class="btn primary" style="margin-top:12px; width:100%;" :disabled="loading || !cart.items.length" @click="validateAddress">
            {{ loading ? 'Validating…' : 'Continue to Order Summary' }}
          </button>

          <div v-if="addressValidation?.isValid" class="card" style="margin-top:10px; background:#ecfeff; border-color:#a5f3fc;">
            Address validated.
          </div>
        </div>

        <div v-else-if="step === 2" class="card">
          <div style="font-weight:900; margin-bottom:10px;">2) Order Summary (split by vendor)</div>
          <div class="muted">This step previews how a single cart becomes multiple vendor orders.</div>
          <label style="display:block; margin-top:12px;">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Coupon code (optional)</div>
            <input class="input" v-model="couponCode" placeholder="WINTER10" />
          </label>
          <button class="btn primary" style="margin-top:12px; width:100%;" :disabled="loading" @click="loadPreview">
            {{ loading ? 'Loading…' : 'Load Summary' }}
          </button>
        </div>

        <div v-else class="card">
          <div style="font-weight:900; margin-bottom:10px;">3) Secure Payment (Cashfree modal stub)</div>

          <div v-if="splitPreview.length" style="display:flex; flex-direction:column; gap:10px;">
            <div v-for="s in splitPreview" :key="s.vendorId" class="card" style="background:#f8fafc;">
              <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
                <b>Vendor #{{ s.vendorId }}</b>
                <span class="pill">{{ s.currency }} {{ s.orderTotalAfterDiscount ?? s.orderTotal }}</span>
              </div>
              <div class="muted" style="font-size:12px; margin-top:6px;">
                Subtotal {{ s.subtotal }}
                + shipping {{ (s.orderTotal - s.subtotal).toFixed(2) }}
                <span v-if="s.discountTotal"> - discount {{ s.discountTotal }}</span>
              </div>
              <ul style="margin:10px 0 0; padding-left:18px;">
                <li v-for="it in s.items" :key="it.productId">
                  {{ it.productName || `Product ${it.productId}` }} × {{ it.quantity }} ({{ it.unitPrice }})
                </li>
              </ul>
            </div>
          </div>

          <button class="btn primary" style="margin-top:12px; width:100%;" :disabled="loading || !cart.items.length" @click="submitPayment">
            {{ loading ? 'Creating session…' : 'Pay with Cashfree' }}
          </button>

          <div v-if="paymentSessionId" class="card" style="margin-top:10px; background:#f0fdf4; border-color:#86efac;">
            <div><b>Cashfree Payment Session:</b> {{ paymentSessionId }}</div>
            <div class="muted" style="margin-top:6px;">Integrate Cashfree Checkout modal in this step using the session id.</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

