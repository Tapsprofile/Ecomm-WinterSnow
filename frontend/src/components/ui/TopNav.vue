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
  <div class="topbar">
    <div class="container" style="display:flex; align-items:center; gap:12px;">
      <RouterLink to="/" style="font-weight:800; letter-spacing:-.02em;">WinterSnow</RouterLink>

      <div style="position:relative; flex:1; max-width:620px;">
        <form @submit.prevent="onSearchSubmit" style="display:flex; gap:8px;">
          <input class="input" v-model="q" placeholder="Search Winter gear (size, material, etc.)" />
          <button class="btn primary" type="submit">Search</button>
        </form>
        <div v-if="open && suggestions.length" class="card" style="position:absolute; top:52px; left:0; right:0; padding:10px;">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Suggestions</div>
          <div style="display:flex; flex-direction:column; gap:6px;">
            <button v-for="s in suggestions" :key="s" class="btn" style="text-align:left;" @click="pickSuggestion(s)">{{ s }}</button>
          </div>
        </div>
      </div>

      <RouterLink to="/checkout" class="pill">Cart: {{ cart.totalItems }}</RouterLink>
      <ThemeToggle />
      <RouterLink v-if="roleLink" :to="roleLink.to" class="pill">{{ roleLink.label }}</RouterLink>
      <RouterLink v-if="!auth.isAuthenticated" to="/login" class="pill">Login</RouterLink>
      <button v-else class="btn" @click="auth.logout()">Logout</button>
    </div>
  </div>
</template>

