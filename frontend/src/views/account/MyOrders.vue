<script setup>
import { onMounted, ref } from 'vue'
import TopNav from '../../components/ui/TopNav.vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const orders = ref([])

async function load() {
  loading.value = true
  error.value = null
  try {
    orders.value = await apiGet('/api/account/me/orders')
  } catch (e) {
    error.value = e?.message || 'Failed to load orders'
  } finally {
    loading.value = false
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
          <div class="fw-bold">Orders</div>
          <div class="text-secondary small">Your order history (split by vendor orders).</div>
        </div>
        <RouterLink class="btn ws-btn-outline btn-sm" to="/account">Back</RouterLink>
      </div>
    </div>

    <div v-if="error" class="alert alert-danger mt-3" role="alert">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="text-secondary mt-3">Loading…</div>

    <div v-if="!loading" class="card ws-card mt-3">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table align-middle">
            <thead>
              <tr>
                <th>Order</th>
                <th>Status</th>
                <th>Vendor</th>
                <th>Items</th>
                <th>Total</th>
                <th>Created</th>
                <th class="text-end"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="o in orders" :key="o.orderId">
                <td class="fw-semibold">#{{ o.orderId }}</td>
                <td><span class="badge text-bg-secondary">{{ o.status }}</span></td>
                <td>#{{ o.vendorId }}</td>
                <td>{{ o.itemsCount }}</td>
                <td>{{ o.currency }} {{ o.orderTotal }}</td>
                <td class="text-secondary small">{{ new Date(o.createdOnUtc).toLocaleString() }}</td>
                <td class="text-end">
                  <RouterLink class="btn ws-btn-dark btn-sm" :to="`/account/orders/${o.orderId}`">Details</RouterLink>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div v-if="!orders.length" class="text-secondary">No orders yet.</div>
      </div>
    </div>
  </div>
</template>

