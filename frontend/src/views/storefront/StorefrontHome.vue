<script setup>
import { onMounted, ref } from 'vue'
import TopNav from '../../components/ui/TopNav.vue'
import ProductCard from '../../components/ui/ProductCard.vue'
import HeroCarousel from '../../components/storefront/HeroCarousel.vue'
import CategoryStrip from '../../components/storefront/CategoryStrip.vue'
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
    <div class="d-flex justify-content-between align-items-center gap-3 flex-wrap">
      <div>
        <div class="ws-section-kicker mb-1">Marketplace storefront</div>
        <div class="ws-section-title">WinterSnow</div>
        <div class="text-secondary small mt-1">Fluxstore-style layout + our marketplace APIs</div>
      </div>
      <RouterLink to="/search" class="btn ws-btn-dark btn-sm">
        <i class="bi bi-search me-1" />
        Browse
      </RouterLink>
    </div>

    <div class="mt-3">
      <HeroCarousel />
    </div>

    <div class="card ws-card mt-3">
      <div class="card-body">
        <div class="fw-bold">Shop by category</div>
        <CategoryStrip />
      </div>
    </div>

    <div v-if="error" class="alert alert-danger mt-3" role="alert">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="text-secondary mt-3">Loading…</div>

    <section v-if="!loading" class="mt-4">
      <div class="d-flex justify-content-between align-items-center">
        <h3 class="h5 mb-0 ws-section-title">Top Picks</h3>
        <RouterLink class="btn ws-btn-outline btn-sm" to="/search">See all</RouterLink>
      </div>
      <div class="row g-3 row-cols-2 row-cols-lg-4 mt-1">
        <div v-for="p in data.topPicks" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>

    <section v-if="!loading" class="mt-4">
      <div class="d-flex justify-content-between align-items-center">
        <h3 class="h5 mb-0 ws-section-title">New Arrivals</h3>
        <RouterLink class="btn ws-btn-outline btn-sm" to="/search">See all</RouterLink>
      </div>
      <div class="row g-3 row-cols-2 row-cols-lg-4 mt-1">
        <div v-for="p in data.newArrivals" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>

    <section v-if="!loading" class="mt-4 mb-4">
      <div class="d-flex justify-content-between align-items-center">
        <h3 class="h5 mb-0 ws-section-title">Winter Collections</h3>
        <RouterLink class="btn ws-btn-outline btn-sm" to="/search">See all</RouterLink>
      </div>
      <div class="row g-3 row-cols-2 row-cols-lg-4 mt-1">
        <div v-for="p in data.winterCollections" :key="p.productId" class="col">
          <ProductCard :item="p" />
        </div>
      </div>
    </section>
  </div>
</template>

