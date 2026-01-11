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
const paymentSessions = ref([])
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
    paymentSessions.value = []
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
    paymentSessions.value = res.paymentSessions || []
  } catch (e) {
    error.value = e?.message || 'Checkout submit failed'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <TopNav />
  <div class="container py-3">
    <div class="card ws-card">
      <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
        <div>
          <div class="ws-section-kicker mb-1">3-step checkout</div>
          <h2 class="ws-section-title mb-0">Checkout</h2>
        </div>
        <span class="badge text-bg-secondary">Step {{ step }}/3</span>
      </div>
    </div>

    <div v-if="error" class="alert alert-danger mt-3" role="alert">
      <b>Error:</b> {{ error }}
    </div>

    <div class="row g-3 align-items-start mt-1">
      <div class="col-12 col-lg-6">
        <div class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-2">Cart</div>
            <div v-if="!cart.items.length" class="text-secondary">Cart is empty.</div>

            <div v-else class="d-flex flex-column gap-2">
              <div v-for="i in cart.items" :key="`${i.productId}:${i.variantId}`" class="card ws-card">
                <div class="card-body d-flex justify-content-between gap-3 align-items-center flex-wrap">
                  <div>
                    <b>{{ i.name || `Product ${i.productId}` }}</b>
                    <div class="text-secondary small">Variant: {{ i.variantId ?? 'default' }}</div>
                  </div>
                  <div class="d-flex gap-2 align-items-center">
                    <input class="form-control form-control-sm" style="width: 90px;" type="number" min="1" v-model.number="i.quantity" />
                    <button class="btn btn-outline-danger btn-sm" type="button" @click="cart.removeItem(i.productId, i.variantId)">Remove</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-lg-6">
        <div v-if="step === 1" class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-3">1) Shipping Address (with validation)</div>

            <div class="row g-2">
              <div class="col-6">
                <label class="form-label small text-secondary">Full name</label>
                <input class="form-control" v-model="address.fullName" />
              </div>
              <div class="col-6">
                <label class="form-label small text-secondary">Phone</label>
                <input class="form-control" v-model="address.phone" />
              </div>
            </div>

            <div class="mt-2">
              <label class="form-label small text-secondary">Address line 1</label>
              <input class="form-control" v-model="address.line1" />
            </div>
            <div class="mt-2">
              <label class="form-label small text-secondary">Address line 2</label>
              <input class="form-control" v-model="address.line2" />
            </div>

            <div class="row g-2 mt-1">
              <div class="col-6">
                <label class="form-label small text-secondary">City</label>
                <input class="form-control" v-model="address.city" />
              </div>
              <div class="col-6">
                <label class="form-label small text-secondary">State</label>
                <input class="form-control" v-model="address.state" />
              </div>
            </div>
            <div class="row g-2 mt-1">
              <div class="col-6">
                <label class="form-label small text-secondary">Postal code</label>
                <input class="form-control" v-model="address.postalCode" />
              </div>
              <div class="col-6">
                <label class="form-label small text-secondary">Country</label>
                <input class="form-control" v-model="address.countryCode" />
              </div>
            </div>

            <button class="btn ws-btn-dark w-100 mt-3" type="button" :disabled="loading || !cart.items.length" @click="validateAddress">
              {{ loading ? 'Validating…' : 'Continue to Order Summary' }}
            </button>

            <div v-if="addressValidation?.isValid" class="alert alert-success mt-3 mb-0" role="alert">
              Address validated.
            </div>
          </div>
        </div>

        <div v-else-if="step === 2" class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-2">2) Order Summary (split by vendor)</div>
            <div class="text-secondary small">This previews how a single cart becomes multiple vendor orders.</div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Coupon code (optional)</label>
              <input class="form-control" v-model="couponCode" placeholder="WINTER10" />
            </div>

            <button class="btn ws-btn-dark w-100 mt-3" type="button" :disabled="loading" @click="loadPreview">
              {{ loading ? 'Loading…' : 'Load Summary' }}
            </button>
          </div>
        </div>

        <div v-else class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-3">3) Secure Payment (Cashfree modal stub)</div>

            <div v-if="splitPreview.length" class="d-flex flex-column gap-2">
              <div v-for="s in splitPreview" :key="s.vendorId" class="card ws-card">
                <div class="card-body">
                  <div class="d-flex justify-content-between align-items-center gap-2 flex-wrap">
                    <b>Vendor #{{ s.vendorId }}</b>
                    <span class="badge text-bg-secondary">{{ s.currency }} {{ s.orderTotalAfterDiscount ?? s.orderTotal }}</span>
                  </div>
                  <div class="text-secondary small mt-2">
                    Subtotal {{ s.subtotal }}
                    + shipping {{ (s.orderTotal - s.subtotal).toFixed(2) }}
                    <span v-if="s.discountTotal"> - discount {{ s.discountTotal }}</span>
                  </div>
                  <ul class="mt-2 mb-0">
                    <li v-for="it in s.items" :key="it.productId">
                      {{ it.productName || `Product ${it.productId}` }} × {{ it.quantity }} ({{ it.unitPrice }})
                    </li>
                  </ul>
                </div>
              </div>
            </div>

            <button class="btn ws-btn-dark w-100 mt-3" type="button" :disabled="loading || !cart.items.length" @click="submitPayment">
              {{ loading ? 'Creating session…' : 'Pay with Cashfree' }}
            </button>

            <div v-if="paymentSessions.length" class="alert alert-success mt-3 mb-0" role="alert">
              <div class="fw-bold mb-2">Payment sessions created</div>
              <ul class="mb-0">
                <li v-for="s in paymentSessions" :key="s.sessionId">
                  <b>{{ s.providerDisplayName }}</b> ({{ s.providerSystemName }}): {{ s.sessionId }}
                  <span class="text-secondary small"> • Orders: {{ (s.orderIds || []).join(', ') }}</span>
                </li>
              </ul>
              <div class="small mt-2">
                Launch the appropriate gateway modal using its session id. (Stub sessions right now.)
              </div>
            </div>
            <div v-else-if="paymentSessionId" class="alert alert-success mt-3 mb-0" role="alert">
              <div><b>Payment Session:</b> {{ paymentSessionId }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

