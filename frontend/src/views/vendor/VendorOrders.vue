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
  <div class="card" style="margin-top:12px;">
    <div style="display:flex; justify-content:space-between; gap:10px; align-items:center; flex-wrap:wrap;">
      <div>
        <div style="font-weight:900;">Order Fulfillment</div>
        <div class="muted">Filters: Awaiting Pickup, Shipped, Completed.</div>
      </div>
      <select class="select" style="width:240px;" v-model="status">
        <option value="">All</option>
        <option value="AwaitingPickup">Awaiting Pickup</option>
        <option value="Shipped">Shipped</option>
        <option value="Completed">Completed</option>
      </select>
    </div>
  </div>

  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <table class="table">
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
          <td><b>#{{ o.orderId }}</b></td>
          <td><span class="pill">{{ o.status }}</span></td>
          <td>{{ o.currency }} {{ o.total }}</td>
          <td class="muted">{{ new Date(o.createdOnUtc).toLocaleString() }}</td>
        </tr>
      </tbody>
    </table>
    <div v-if="!orders.length" class="muted" style="margin-top:10px;">No orders found.</div>
  </div>
</template>

