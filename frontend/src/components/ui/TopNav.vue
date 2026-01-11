<script setup>
import { computed, ref, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useCartStore } from '../../stores/cart'
import { useAuthStore } from '../../stores/auth'
import ThemeToggle from './ThemeToggle.vue'
import { apiGet } from '../../lib/api'
import { useNotificationsStore } from '../../stores/notifications'
import { onMounted, onUnmounted } from 'vue'

const router = useRouter()
const route = useRoute()
const cart = useCartStore()
const auth = useAuthStore()
const notifications = useNotificationsStore()

const navRef = ref(null)

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

function onScroll() {
  const el = navRef.value
  if (!el) return
  el.classList.toggle('ws-nav-shadow', window.scrollY > 50)
}

let poll = null
onMounted(async () => {
  if (auth.isAuthenticated) await notifications.refresh()
  poll = setInterval(() => {
    if (auth.isAuthenticated) notifications.refresh().catch(() => {})
  }, 20000)

  window.addEventListener('scroll', onScroll, { passive: true })
  onScroll()
})

onUnmounted(() => {
  if (poll) clearInterval(poll)
  window.removeEventListener('scroll', onScroll)
})
</script>

<template>
  <nav ref="navRef" class="navbar navbar-expand-lg sticky-top ws-nav-glass">
    <div class="container">
      <RouterLink to="/store" class="navbar-brand fw-bold">WinterSnow</RouterLink>

      <div class="d-flex flex-grow-1 align-items-center gap-2 position-relative" style="max-width: 720px;">
        <form class="flex-grow-1" @submit.prevent="onSearchSubmit">
          <div class="input-group">
            <span class="input-group-text"><i class="bi bi-search" /></span>
            <input class="form-control" v-model="q" placeholder="Search (size, material, etc.)" />
            <button class="btn ws-btn-dark" type="submit">Search</button>
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
        <RouterLink to="/checkout" class="btn ws-btn-outline btn-sm">
          <i class="bi bi-bag me-1" />
          Cart
          <span class="badge text-bg-secondary ms-2">{{ cart.totalItems }}</span>
        </RouterLink>

        <div v-if="auth.isAuthenticated" class="dropdown">
          <button
            class="btn ws-btn-outline btn-sm dropdown-toggle"
            type="button"
            data-bs-toggle="dropdown"
            aria-expanded="false"
            @click="notifications.refresh().catch(() => {})"
          >
            <i class="bi bi-bell me-1" />
            <span v-if="notifications.unread" class="badge text-bg-danger ms-1">{{ notifications.unread }}</span>
          </button>
          <div class="dropdown-menu dropdown-menu-end p-2" style="min-width: 360px;">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <div class="fw-bold">Notifications</div>
              <button class="btn btn-link btn-sm" type="button" @click="notifications.markAllRead()">Mark all read</button>
            </div>

            <div v-if="!notifications.items.length" class="text-secondary small p-2">No notifications.</div>

            <div v-else class="list-group">
              <button
                v-for="n in notifications.items"
                :key="n.id"
                type="button"
                class="list-group-item list-group-item-action"
                :class="{ 'fw-semibold': !n.isRead }"
                @click="notifications.markRead(n.id)"
              >
                <div class="d-flex justify-content-between gap-2">
                  <div>
                    <div>{{ n.title }}</div>
                    <div class="text-secondary small">{{ n.body }}</div>
                  </div>
                  <div class="text-secondary small">{{ new Date(n.createdOnUtc).toLocaleTimeString() }}</div>
                </div>
              </button>
            </div>
          </div>
        </div>

        <ThemeToggle />

        <RouterLink v-if="roleLink" :to="roleLink.to" class="btn ws-btn-outline btn-sm">
          <i class="bi bi-grid me-1" />
          {{ roleLink.label }}
        </RouterLink>

        <RouterLink v-if="!auth.isAuthenticated" to="/login" class="btn ws-btn-danger btn-sm">
          Login
        </RouterLink>
        <button v-else class="btn ws-btn-outline btn-sm" type="button" @click="auth.logout()">
          Logout
        </button>
      </div>
    </div>
  </nav>
</template>

