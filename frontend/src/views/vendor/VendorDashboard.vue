<script setup>
import { onMounted, ref } from 'vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const data = ref({ revenueToday: 0, revenueThisWeek: 0, averageOrderValue: 0, topWinterProducts: [] })

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet('/api/vendor/dashboard/overview')
  } catch (e) {
    error.value = e?.message || 'Failed to load vendor overview'
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

  <div v-if="!loading" class="row" style="margin-top:12px; align-items:stretch;">
    <div class="col card">
      <div class="muted" style="font-size:12px;">Revenue Today</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.revenueToday }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Revenue (7 days)</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.revenueThisWeek }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Average Order Value</div>
      <div style="font-size:26px; font-weight:900; margin-top:6px;">INR {{ data.averageOrderValue }}</div>
    </div>
  </div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <div style="display:flex; justify-content:space-between; align-items:center;">
      <div style="font-weight:900;">Top Winter Products</div>
      <span class="pill">This week</span>
    </div>

    <div v-if="!data.topWinterProducts.length" class="muted" style="margin-top:10px;">No sales yet.</div>

    <div v-else style="margin-top:10px; display:flex; flex-direction:column; gap:10px;">
      <div v-for="p in data.topWinterProducts" :key="p.productId" class="card" style="background:#f8fafc;">
        <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
          <div>
            <b>{{ p.name }}</b>
            <div class="muted" style="font-size:12px;">Units sold: {{ p.unitsSold }}</div>
          </div>
          <div class="pill">INR {{ p.revenue }}</div>
        </div>
        <div style="height:8px; background:#e2e8f0; border-radius:999px; overflow:hidden; margin-top:10px;">
          <div
            :style="{
              height: '100%',
              width: Math.min(100, (p.revenue / (data.topWinterProducts[0]?.revenue || 1)) * 100) + '%',
              background: '#2563eb'
            }"
          />
        </div>
      </div>
    </div>
  </div>
</template>

