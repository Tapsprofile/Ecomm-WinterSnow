<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import TopNav from '../../components/ui/TopNav.vue'
import ProductCard from '../../components/ui/ProductCard.vue'
import { apiGet } from '../../lib/api'

const route = useRoute()
const router = useRouter()

const q = ref(route.query.q?.toString() || '')
const material = ref(route.query.material?.toString() || '')
const size = ref(route.query.size?.toString() || '')
const color = ref(route.query.color?.toString() || '')
const minPrice = ref(route.query.minPrice?.toString() || '')
const maxPrice = ref(route.query.maxPrice?.toString() || '')

const data = ref({ items: [], total: 0, facets: { sizes: [], colors: [], materials: [] } })
const loading = ref(false)
const error = ref(null)

function syncQuery() {
  router.replace({
    name: 'search',
    query: {
      q: q.value || undefined,
      material: material.value || undefined,
      size: size.value || undefined,
      color: color.value || undefined,
      minPrice: minPrice.value || undefined,
      maxPrice: maxPrice.value || undefined
    }
  })
}

const queryString = computed(() => {
  const params = new URLSearchParams()
  if (q.value) params.set('q', q.value)
  if (material.value) params.set('material', material.value)
  if (size.value) params.set('size', size.value)
  if (color.value) params.set('color', color.value)
  if (minPrice.value) params.set('minPrice', minPrice.value)
  if (maxPrice.value) params.set('maxPrice', maxPrice.value)
  return params.toString()
})

async function load() {
  loading.value = true
  error.value = null
  try {
    data.value = await apiGet(`/api/storefront/search?${queryString.value}`)
  } catch (e) {
    error.value = e?.message || 'Search failed'
  } finally {
    loading.value = false
  }
}

let t = null
watch([q, material, size, color, minPrice, maxPrice], () => {
  clearTimeout(t)
  t = setTimeout(() => {
    syncQuery()
    load()
  }, 200)
})

onMounted(load)
</script>

<template>
  <TopNav />
  <div class="container">
    <div class="row" style="align-items:flex-start; margin-top:16px;">
      <aside class="card" style="width:320px;">
        <div style="font-weight:800; margin-bottom:10px;">Faceted Filtering</div>

        <label style="display:block; margin-bottom:10px;">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Query</div>
          <input class="input" v-model="q" placeholder="e.g. jacket, gloves" />
        </label>

        <div class="row">
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Min Price</div>
            <input class="input" v-model="minPrice" inputmode="numeric" />
          </label>
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Max Price</div>
            <input class="input" v-model="maxPrice" inputmode="numeric" />
          </label>
        </div>

        <label style="display:block; margin-top:10px;">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Material</div>
          <select class="select" v-model="material">
            <option value="">All</option>
            <option v-for="b in data.facets.materials" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
          </select>
        </label>

        <label style="display:block; margin-top:10px;">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Size</div>
          <select class="select" v-model="size">
            <option value="">All</option>
            <option v-for="b in data.facets.sizes" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
          </select>
        </label>

        <label style="display:block; margin-top:10px;">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Color</div>
          <select class="select" v-model="color">
            <option value="">All</option>
            <option v-for="b in data.facets.colors" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
          </select>
        </label>

        <button
          class="btn"
          style="margin-top:12px; width:100%;"
          @click="q=''; material=''; size=''; color=''; minPrice=''; maxPrice='';"
        >
          Clear filters
        </button>
      </aside>

      <main class="col">
        <div class="card" style="display:flex; justify-content:space-between; align-items:center;">
          <div>
            <div style="font-weight:800;">Results</div>
            <div class="muted">{{ data.total }} products</div>
          </div>
          <RouterLink to="/" class="btn">Back to home</RouterLink>
        </div>

        <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
          <b>Error:</b> {{ error }}
        </div>
        <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

        <div v-if="!loading" class="grid" style="margin-top:12px;">
          <ProductCard v-for="p in data.items" :key="p.productId" :item="p" />
        </div>
      </main>
    </div>
  </div>
</template>

