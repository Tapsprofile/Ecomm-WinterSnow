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
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1">
    <div class="col-12 col-md-4">
      <div class="card">
        <div class="card-body">
          <div class="text-secondary small">Status</div>
          <div class="h5 fw-bold mt-1 mb-0">{{ data.status }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card">
        <div class="card-body">
          <div class="text-secondary small">Uptime</div>
          <div class="h5 fw-bold mt-1 mb-0">{{ Math.round(data.uptimeSeconds) }}s</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card">
        <div class="card-body">
          <div class="text-secondary small">API latency</div>
          <div class="mt-2 d-flex gap-2 flex-wrap">
            <span class="badge text-bg-secondary">p50 {{ data.apiResponseP50Ms.toFixed(1) }}ms</span>
            <span class="badge text-bg-secondary">p95 {{ data.apiResponseP95Ms.toFixed(1) }}ms</span>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card mt-3">
    <div class="card-body">
      <div class="fw-bold">Recent errors (from .NET middleware)</div>
      <div v-if="!data.recentErrors?.length" class="text-secondary mt-2">No errors captured.</div>
      <ul v-else class="mt-2 mb-0">
        <li v-for="e in data.recentErrors" :key="e" class="text-secondary small">{{ e }}</li>
      </ul>
    </div>
  </div>
</template>

