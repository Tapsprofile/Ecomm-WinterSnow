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
const postalCode = ref(route.query.postalCode?.toString() || '')

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
      ,postalCode: postalCode.value || undefined
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
  if (postalCode.value) params.set('postalCode', postalCode.value)
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
watch([q, material, size, color, minPrice, maxPrice, postalCode], () => {
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
  <div class="container py-3">
    <div class="row g-3 align-items-start">
      <aside class="col-12 col-lg-3">
        <div class="card">
          <div class="card-body">
            <div class="fw-bold mb-3">Faceted Filtering</div>

            <div class="mb-3">
              <label class="form-label small text-secondary">Query</label>
              <input class="form-control" v-model="q" placeholder="e.g. jacket, gloves" />
            </div>

            <div class="row g-2">
              <div class="col-6">
                <label class="form-label small text-secondary">Min Price</label>
                <input class="form-control" v-model="minPrice" inputmode="numeric" />
              </div>
              <div class="col-6">
                <label class="form-label small text-secondary">Max Price</label>
                <input class="form-control" v-model="maxPrice" inputmode="numeric" />
              </div>
            </div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Delivery postal code (optional)</label>
              <input class="form-control" v-model="postalCode" placeholder="e.g. 560001" />
              <div class="text-secondary small mt-1">Restricted listings only appear when this is set.</div>
            </div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Material</label>
              <select class="form-select" v-model="material">
                <option value="">All</option>
                <option v-for="b in data.facets.materials" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
              </select>
            </div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Size</label>
              <select class="form-select" v-model="size">
                <option value="">All</option>
                <option v-for="b in data.facets.sizes" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
              </select>
            </div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Color</label>
              <select class="form-select" v-model="color">
                <option value="">All</option>
                <option v-for="b in data.facets.colors" :key="b.value" :value="b.value">{{ b.value }} ({{ b.count }})</option>
              </select>
            </div>

            <button class="btn btn-outline-secondary w-100 mt-3" type="button" @click="q=''; material=''; size=''; color=''; minPrice=''; maxPrice='';">
              Clear filters
            </button>
          </div>
        </div>
      </aside>

      <main class="col-12 col-lg-9">
        <div class="card">
          <div class="card-body d-flex justify-content-between align-items-center">
            <div>
              <div class="fw-bold">Results</div>
              <div class="text-secondary small">{{ data.total }} products</div>
            </div>
            <RouterLink to="/" class="btn btn-outline-secondary btn-sm">Back to home</RouterLink>
          </div>
        </div>

        <div v-if="error" class="alert alert-danger mt-3" role="alert">
          <b>Error:</b> {{ error }}
        </div>
        <div v-if="loading" class="text-secondary mt-3">Loading…</div>

        <div v-if="!loading" class="row g-3 row-cols-1 row-cols-sm-2 row-cols-lg-3 mt-1">
          <div v-for="p in data.items" :key="p.productId" class="col">
            <ProductCard :item="p" />
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

