<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import TopNav from '../../components/ui/TopNav.vue'
import { apiGet, apiPost } from '../../lib/api'

const route = useRoute()
const loading = ref(true)
const error = ref(null)
const order = ref(null)

const returnForm = ref({
  open: false,
  orderItemId: null,
  reason: '',
  notes: ''
})

const orderId = computed(() => Number(route.params.orderId))

async function load() {
  loading.value = true
  error.value = null
  try {
    order.value = await apiGet(`/api/account/me/orders/${orderId.value}`)
  } catch (e) {
    error.value = e?.message || 'Failed to load order'
  } finally {
    loading.value = false
  }
}

function openReturn(itemId) {
  returnForm.value = { open: true, orderItemId: itemId, reason: '', notes: '' }
}

async function submitReturn() {
  error.value = null
  try {
    await apiPost('/api/returns', {
      orderId: order.value.orderId,
      orderItemId: returnForm.value.orderItemId,
      reason: returnForm.value.reason,
      notes: returnForm.value.notes || null
    })
    returnForm.value.open = false
    alert('Return request created.')
  } catch (e) {
    error.value = e?.message || 'Failed to create return'
  }
}

onMounted(load)
</script>

<template>
  <TopNav />
  <div class="container py-4">
    <div class="card ws-card">
      <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
        <div>
          <div class="ws-section-kicker mb-1">My account</div>
          <div class="fw-bold">Order #{{ orderId }}</div>
          <div v-if="order" class="text-secondary small">Vendor #{{ order.vendorId }} • {{ order.status }}</div>
        </div>
        <RouterLink class="btn ws-btn-outline btn-sm" to="/account/orders">Back to orders</RouterLink>
      </div>
    </div>

    <div v-if="error" class="alert alert-danger mt-3" role="alert">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="text-secondary mt-3">Loading…</div>

    <div v-if="!loading && order" class="row g-3 mt-1 align-items-start">
      <div class="col-12 col-lg-7">
        <div class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-2">Items</div>
            <div class="d-flex flex-column gap-2">
              <div v-for="it in order.items" :key="it.orderItemId" class="card ws-card">
                <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
                  <div>
                    <div class="fw-semibold">{{ it.productName }}</div>
                    <div class="text-secondary small">Product #{{ it.productId }} • Variant: {{ it.productVariantId ?? 'default' }}</div>
                  </div>
                  <div class="text-end">
                    <div class="fw-bold">{{ order.currency }} {{ it.priceInclTax }}</div>
                    <div class="text-secondary small">{{ it.quantity }} × {{ it.unitPriceInclTax }}</div>
                    <button class="btn ws-btn-outline btn-sm mt-2" type="button" @click="openReturn(it.orderItemId)">
                      Request return
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-lg-5">
        <div class="card ws-card">
          <div class="card-body">
            <div class="fw-bold mb-2">Summary</div>
            <div class="d-flex justify-content-between"><span>Subtotal</span><b>{{ order.currency }} {{ order.subtotal }}</b></div>
            <div class="d-flex justify-content-between"><span>Discount</span><b>{{ order.currency }} {{ order.discountTotal }}</b></div>
            <div class="d-flex justify-content-between"><span>Shipping</span><b>{{ order.currency }} {{ order.shippingFee }}</b></div>
            <div class="d-flex justify-content-between"><span>Tax</span><b>{{ order.currency }} {{ order.taxTotal }}</b></div>
            <hr />
            <div class="d-flex justify-content-between"><span>Total</span><b>{{ order.currency }} {{ order.orderTotal }}</b></div>
            <div class="text-secondary small mt-2">Created: {{ new Date(order.createdOnUtc).toLocaleString() }}</div>
          </div>
        </div>

        <div class="card ws-card mt-3">
          <div class="card-body">
            <div class="fw-bold mb-2">Shipping address</div>
            <div class="small">
              <div class="fw-semibold">{{ order.shippingAddress.fullName }}</div>
              <div>{{ order.shippingAddress.line1 }}</div>
              <div v-if="order.shippingAddress.line2">{{ order.shippingAddress.line2 }}</div>
              <div>{{ order.shippingAddress.city }}, {{ order.shippingAddress.state }} {{ order.shippingAddress.postalCode }}</div>
              <div>{{ order.shippingAddress.countryCode }}</div>
              <div v-if="order.shippingAddress.phone" class="text-secondary">Phone: {{ order.shippingAddress.phone }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Return modal (simple inline) -->
    <div
      v-if="returnForm.open"
      class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center p-3"
      style="background: rgba(0,0,0,.72); z-index: 2000;"
      @click.self="returnForm.open = false"
    >
      <div class="card ws-card" style="width: min(640px, 95vw);">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center">
            <div class="fw-bold">Create return request</div>
            <button class="btn ws-btn-outline btn-sm" type="button" @click="returnForm.open = false">
              Close
            </button>
          </div>

          <div class="mt-3">
            <label class="form-label">Reason</label>
            <input class="form-control" v-model="returnForm.reason" placeholder="e.g. Wrong size / Damaged / Not as described" />
          </div>
          <div class="mt-2">
            <label class="form-label">Notes (optional)</label>
            <textarea class="form-control" rows="3" v-model="returnForm.notes" />
          </div>

          <button class="btn ws-btn-dark w-100 mt-3" type="button" :disabled="!returnForm.reason" @click="submitReturn">
            Submit return request
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

