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
  <div class="container">
    <div class="card" style="padding:18px; margin-top:16px;">
      <div style="display:flex; justify-content:space-between; gap:12px; align-items:flex-start;">
        <div>
          <div class="pill">Mobile-first • Fast</div>
          <h1 style="margin:10px 0 6px; line-height:1.1;">Winter Snow</h1>
          <div class="muted">Top Picks, New Arrivals, and Winter Collections.</div>
        </div>
        <RouterLink to="/search" class="btn primary">Browse</RouterLink>
      </div>
    </div>

    <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

    <section v-if="!loading" style="margin-top:18px;">
      <h3 style="margin:0 0 10px;">Top Picks</h3>
      <div class="grid">
        <ProductCard v-for="p in data.topPicks" :key="p.productId" :item="p" />
      </div>
    </section>

    <section v-if="!loading" style="margin-top:18px;">
      <h3 style="margin:0 0 10px;">New Arrivals</h3>
      <div class="grid">
        <ProductCard v-for="p in data.newArrivals" :key="p.productId" :item="p" />
      </div>
    </section>

    <section v-if="!loading" style="margin-top:18px; margin-bottom:28px;">
      <h3 style="margin:0 0 10px;">Winter Collections</h3>
      <div class="grid">
        <ProductCard v-for="p in data.winterCollections" :key="p.productId" :item="p" />
      </div>
    </section>
  </div>
</template>

