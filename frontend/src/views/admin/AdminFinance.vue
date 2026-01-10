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
  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="row" style="margin-top:12px;">
    <div class="col card">
      <div class="muted" style="font-size:12px;">Platform revenue (gross)</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.platformRevenueGross }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Total commissions earned</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.totalCommissionsEarned }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Orders</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">{{ data.ordersCount }}</div>
    </div>
  </div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <div style="font-weight:900;">Bulk payouts (Cashfree API)</div>
    <div class="muted" style="margin-top:6px;">
      This UI is ready to trigger bulk payouts, but the backend Cashfree payout API integration is intentionally stubbed in this scaffold.
    </div>
    <button class="btn" style="margin-top:12px;" disabled>Trigger bulk payouts (stub)</button>
  </div>
</template>

