<script setup>
import { onMounted, ref } from 'vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const data = ref({ totalSales: 0, nextPayoutDateUtc: null })

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet('/api/vendor/payouts')
  } catch (e) {
    error.value = e?.message || 'Failed to load payouts'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="card ws-card mt-3">
    <div class="card-body">
      <div class="ws-section-kicker mb-1">Payouts & ledger</div>
      <div class="fw-bold">Vendor finance</div>
      <div class="text-secondary small">Total sales minus platform commission and next payout date.</div>
    </div>
  </div>

  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1">
    <div class="col-12 col-md-6">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Total Sales (net)</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.totalSales }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-6">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Next Payout Date</div>
          <div class="h6 fw-bold mt-1 mb-0">
            {{ data.nextPayoutDateUtc ? new Date(data.nextPayoutDateUtc).toLocaleDateString() : '—' }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

