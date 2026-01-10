<script setup>
import { computed, ref, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useCartStore } from '../../stores/cart'
import { useAuthStore } from '../../stores/auth'
import ThemeToggle from './ThemeToggle.vue'
import { apiGet } from '../../lib/api'

const router = useRouter()
const route = useRoute()
const cart = useCartStore()
const auth = useAuthStore()

const q = ref(route.query.q?.toString() || '')
watch(
  () => route.query.q,
  (v) => {
    q.value = v?.toString() || ''
  }
)

const suggestions = ref([])
const open = ref(false)
let t = null

watch(q, (v) => {
  clearTimeout(t)
  if (!v || v.trim().length < 2) {
    suggestions.value = []
    open.value = false
    return
  }
  t = setTimeout(async () => {
    try {
      suggestions.value = await apiGet(`/api/storefront/search/autocomplete?q=${encodeURIComponent(v)}`)
      open.value = true
    } catch {
      suggestions.value = []
      open.value = false
    }
  }, 200)
})

const roleLink = computed(() => {
  if (!auth.isAuthenticated) return null
  if (auth.role === 'Vendor') return { label: 'Vendor', to: '/vendor' }
  if (auth.role === 'Admin') return { label: 'Admin', to: '/admin' }
  return null
})

function onSearchSubmit() {
  router.push({ name: 'search', query: { q: q.value || undefined } })
  open.value = false
}

function pickSuggestion(s) {
  q.value = s
  onSearchSubmit()
}
</script>

<template>
  <nav class="navbar navbar-expand-lg sticky-top bg-body border-bottom">
    <div class="container">
      <RouterLink to="/" class="navbar-brand fw-bold">WinterSnow</RouterLink>

      <div class="d-flex flex-grow-1 align-items-center gap-2 position-relative" style="max-width: 720px;">
        <form class="flex-grow-1" @submit.prevent="onSearchSubmit">
          <div class="input-group">
            <span class="input-group-text"><i class="bi bi-search" /></span>
            <input class="form-control" v-model="q" placeholder="Search (size, material, etc.)" />
            <button class="btn btn-primary" type="submit">Search</button>
          </div>
        </form>

        <div
          v-if="open && suggestions.length"
          class="position-absolute start-0 end-0"
          style="top: 46px; z-index: 1050;"
        >
          <div class="card">
            <div class="small text-secondary mb-2">Suggestions</div>
            <div class="list-group">
              <button
                v-for="s in suggestions"
                :key="s"
                type="button"
                class="list-group-item list-group-item-action"
                @click="pickSuggestion(s)"
              >
                {{ s }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="d-flex align-items-center gap-2 ms-2">
        <RouterLink to="/checkout" class="btn btn-outline-secondary btn-sm">
          <i class="bi bi-bag me-1" />
          Cart
          <span class="badge text-bg-secondary ms-2">{{ cart.totalItems }}</span>
        </RouterLink>

        <ThemeToggle />

        <RouterLink v-if="roleLink" :to="roleLink.to" class="btn btn-outline-primary btn-sm">
          <i class="bi bi-grid me-1" />
          {{ roleLink.label }}
        </RouterLink>

        <RouterLink v-if="!auth.isAuthenticated" to="/login" class="btn btn-primary btn-sm">
          Login
        </RouterLink>
        <button v-else class="btn btn-outline-secondary btn-sm" type="button" @click="auth.logout()">
          Logout
        </button>
      </div>
    </div>
  </nav>
</template>

