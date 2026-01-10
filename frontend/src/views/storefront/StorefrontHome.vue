<script setup>
import { onMounted, ref } from 'vue'
import TopNav from '../../components/ui/TopNav.vue'
import ProductCard from '../../components/ui/ProductCard.vue'
import { apiGet } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const data = ref({ topPicks: [], newArrivals: [], winterCollections: [] })

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet('/api/storefront/home')
  } catch (e) {
    error.value = e?.message || 'Failed to load homepage'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <TopNav />
  <div class="container py-3">
    <div class="card">
      <div class="card-body d-flex justify-content-between align-items-start gap-3 flex-wrap">
        <div>
          <span class="badge text-bg-secondary">Mobile-first • Fast</span>
          <h1 class="h3 mt-3 mb-1">Winter Snow</h1>
          <div class="text-secondary">Top Picks, New Arrivals, and Winter Collections.</div>
        </div>
        <RouterLink to="/search" class="btn btn-primary">Browse</RouterLink>
      </div>
    </div>

    <div v-if="error" class="alert alert-danger mt-3" role="alert">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="text-secondary mt-3">Loading…</div>

    <section v-if="!loading" class="mt-4">
      <h3 class="h5 mb-3">Top Picks</h3>
      <div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-4">
        <div v-for="p in data.topPicks" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>

    <section v-if="!loading" class="mt-4">
      <h3 class="h5 mb-3">New Arrivals</h3>
      <div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-4">
        <div v-for="p in data.newArrivals" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>

    <section v-if="!loading" class="mt-4 mb-4">
      <h3 class="h5 mb-3">Winter Collections</h3>
      <div class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-4">
        <div v-for="p in data.winterCollections" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>
  </div>
</template>

