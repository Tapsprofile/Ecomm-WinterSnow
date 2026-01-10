<script setup>
import { onMounted, ref } from 'vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const data = ref({ status: '—', uptimeSeconds: 0, apiResponseP50Ms: 0, apiResponseP95Ms: 0, recentErrors: [] })

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet('/api/admin/system/health')
  } catch (e) {
    error.value = e?.message || 'Failed to load system health'
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
      <div class="muted" style="font-size:12px;">Status</div>
      <div style="font-size:22px; font-weight:900; margin-top:6px;">{{ data.status }}</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">Uptime</div>
      <div style="font-size:22px; font-weight:900; margin-top:6px;">{{ Math.round(data.uptimeSeconds) }}s</div>
    </div>
    <div class="col card">
      <div class="muted" style="font-size:12px;">API latency</div>
      <div style="font-size:14px; margin-top:6px;">
        <span class="pill">p50 {{ data.apiResponseP50Ms.toFixed(1) }}ms</span>
        <span class="pill" style="margin-left:6px;">p95 {{ data.apiResponseP95Ms.toFixed(1) }}ms</span>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <div style="font-weight:900;">Recent errors (from .NET middleware)</div>
    <div v-if="!data.recentErrors?.length" class="muted" style="margin-top:10px;">No errors captured.</div>
    <ul v-else style="margin-top:10px;">
      <li v-for="e in data.recentErrors" :key="e" class="muted" style="margin-bottom:6px;">{{ e }}</li>
    </ul>
  </div>
</template>

