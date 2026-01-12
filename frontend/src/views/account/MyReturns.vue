<script setup>
import { onMounted, ref } from 'vue'
import TopNav from '../../components/ui/TopNav.vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const items = ref([])

async function load() {
  loading.value = true
  error.value = null
  try {
    items.value = await apiGet('/api/returns/me')
  } catch (e) {
    error.value = e?.message || 'Failed to load returns'
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
          <div class="fw-bold">Returns</div>
          <div class="text-secondary small">Track return requests and vendor decisions.</div>
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
                <th>Return</th>
                <th>Order</th>
                <th>Order Item</th>
                <th>Vendor</th>
                <th>Status</th>
                <th>Reason</th>
                <th>Created</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in items" :key="r.returnId">
                <td class="fw-semibold">#{{ r.returnId }}</td>
                <td>#{{ r.orderId }}</td>
                <td>#{{ r.orderItemId }}</td>
                <td>#{{ r.vendorId }}</td>
                <td><span class="badge text-bg-secondary">{{ r.status }}</span></td>
                <td>{{ r.reason }}</td>
                <td class="text-secondary small">{{ new Date(r.createdOnUtc).toLocaleString() }}</td>
              </tr>
            </tbody>
          </table>
        </div>
        <div v-if="!items.length" class="text-secondary">No returns yet.</div>
      </div>
    </div>
  </div>
</template>

