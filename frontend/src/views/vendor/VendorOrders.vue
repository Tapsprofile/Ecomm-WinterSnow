<script setup>
import { onMounted, ref, watch } from 'vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const status = ref('')
const orders = ref([])

async function load() {
  loading.value = true
  error.value = null
  try {
    const qs = status.value ? `?status=${encodeURIComponent(status.value)}` : ''
    const res = await apiGet(`/api/vendor/orders${qs}`)
    orders.value = res.orders || []
  } catch (e) {
    error.value = e?.message || 'Failed to load orders'
  } finally {
    loading.value = false
  }
}

onMounted(load)
watch(status, load)
</script>

<template>
  <div class="card mt-3">
    <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
      <div>
        <div class="fw-bold">Order Fulfillment</div>
        <div class="text-secondary small">Filters: Awaiting Pickup, Shipped, Completed.</div>
      </div>
      <select class="form-select" style="width: 240px;" v-model="status">
        <option value="">All</option>
        <option value="AwaitingPickup">Awaiting Pickup</option>
        <option value="Shipped">Shipped</option>
        <option value="Completed">Completed</option>
      </select>
    </div>
  </div>

  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="card mt-3">
    <div class="card-body">
      <div class="table-responsive">
        <table class="table align-middle">
          <thead>
            <tr>
              <th>Order</th>
              <th>Status</th>
              <th>Total</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="o in orders" :key="o.orderId">
              <td class="fw-semibold">#{{ o.orderId }}</td>
              <td><span class="badge text-bg-secondary">{{ o.status }}</span></td>
              <td>{{ o.currency }} {{ o.total }}</td>
              <td class="text-secondary small">{{ new Date(o.createdOnUtc).toLocaleString() }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="!orders.length" class="text-secondary">No orders found.</div>
    </div>
  </div>
</template>

