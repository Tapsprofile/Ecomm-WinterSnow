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
  <div class="card" style="margin-top:12px;">
    <div style="font-weight:900;">Payouts & Ledger</div>
    <div class="muted">Total sales minus platform commission and next payout date.</div>
  </div>

  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="row" style="margin-top:12px;">
    <div class="col card">
      <div class="muted" style="font-size:12px;">Total Sales (net)</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.totalSales }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Next Payout Date</div>
      <div style="font-size:18px; font-weight:800; margin-top:6px;">
        {{ data.nextPayoutDateUtc ? new Date(data.nextPayoutDateUtc).toLocaleDateString() : '—' }}
      </div>
    </div>
  </div>
</template>

