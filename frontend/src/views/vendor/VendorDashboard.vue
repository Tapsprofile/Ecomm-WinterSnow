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
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1">
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Revenue Today</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.revenueToday }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Revenue (7 days)</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.revenueThisWeek }}</div>
        </div>
      </div>
    </div>
    <div class="col-12 col-md-4">
      <div class="card ws-card">
        <div class="card-body">
          <div class="text-secondary small">Average Order Value</div>
          <div class="h4 fw-bold mt-1 mb-0">INR {{ data.averageOrderValue }}</div>
        </div>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card ws-card mt-3">
    <div class="card-body">
      <div class="d-flex justify-content-between align-items-center">
        <div class="fw-bold">Top Winter Products</div>
        <span class="badge text-bg-secondary">This week</span>
      </div>

      <div v-if="!data.topWinterProducts.length" class="text-secondary mt-3">No sales yet.</div>

      <div v-else class="d-flex flex-column gap-2 mt-3">
        <div v-for="p in data.topWinterProducts" :key="p.productId" class="card ws-card">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center gap-3 flex-wrap">
              <div>
                <b>{{ p.name }}</b>
                <div class="text-secondary small">Units sold: {{ p.unitsSold }}</div>
              </div>
              <span class="badge text-bg-secondary">INR {{ p.revenue }}</span>
            </div>

            <div class="progress mt-3" style="height: 8px;">
              <div
                class="progress-bar"
                :style="{ width: Math.min(100, (p.revenue / (data.topWinterProducts[0]?.revenue || 1)) * 100) + '%' }"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

