<script setup>
import { onMounted, ref } from 'vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const data = ref({ platformRevenueGross: 0, totalCommissionsEarned: 0, ordersCount: 0 })

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet('/api/admin/finance/summary')
  } catch (e) {
    error.value = e?.message || 'Failed to load finance summary'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1">
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Platform revenue (gross)</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.platformRevenueGross }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Total commissions earned</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.totalCommissionsEarned }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Orders</div>
          <div class="h4 fw-bold mt-1 mb-0">{{ data.ordersCount }}</div>
        </div>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card ws-card mt-3">
    <div class="card-body">
      <div class="ws-section-kicker mb-1">Finance</div>
      <div class="fw-bold">Bulk payouts (Cashfree API)</div>
      <div class="text-secondary small mt-1">
        This UI is ready to trigger bulk payouts, but the backend Cashfree payout API integration is intentionally stubbed in this scaffold.
      </div>
      <button class="btn ws-btn-outline mt-3" disabled>Trigger bulk payouts (stub)</button>
    </div>
  </div>
</template>

