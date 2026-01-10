<script setup>
import TopNav from '../../components/ui/TopNav.vue'
import { computed, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../../stores/auth'
import { useThemeStore } from '../../stores/theme'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()
const theme = useThemeStore()

const expectedRole = computed(() => route.meta.loginRole || 'Customer')

const presets = {
  Customer: { email: 'customer@demo.local', password: 'Customer123!' },
  Vendor: { email: 'vendor@demo.local', password: 'Vendor123!' },
  Admin: { email: 'admin@demo.local', password: 'Admin123!' }
}

const email = ref(presets[expectedRole.value].email)
const password = ref(presets[expectedRole.value].password)
const error = ref(null)
const loading = ref(false)

function title() {
  if (expectedRole.value === 'Admin') return 'System Admin Login'
  if (expectedRole.value === 'Vendor') return 'Vendor Login'
  return 'Customer Login'
}

async function submit() {
  error.value = null
  loading.value = true
  try {
    await auth.login(email.value, password.value)

    // Ensure portal-specific login matches the role.
    if (auth.role !== expectedRole.value) {
      auth.logout()
      throw new Error(`This account is not a ${expectedRole.value} account.`)
    }

    // Apply per-role theme preference immediately.
    theme.apply()

    const redirect = route.query.redirect?.toString()
    if (redirect) router.push(redirect)
    else if (auth.role === 'Vendor') router.push('/vendor')
    else if (auth.role === 'Admin') router.push('/admin')
    else router.push('/')
  } catch (e) {
    error.value = e?.message || 'Login failed'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <TopNav />
  <div class="container py-4">
    <div class="card mx-auto" style="max-width: 520px;">
      <div class="card-body">
        <span class="badge text-bg-secondary">{{ expectedRole }} portal</span>
        <h2 class="h4 mt-3 mb-1">{{ title() }}</h2>
        <div class="text-secondary">Use the credentials for the selected portal.</div>

        <div v-if="error" class="alert alert-danger mt-3 mb-0" role="alert">
          <b>Error:</b> {{ error }}
        </div>

        <form class="mt-3" @submit.prevent="submit">
          <div class="mb-3">
            <label class="form-label">Email</label>
            <input class="form-control" v-model="email" autocomplete="username" />
          </div>
          <div class="mb-3">
            <label class="form-label">Password</label>
            <input class="form-control" type="password" v-model="password" autocomplete="current-password" />
          </div>
          <button class="btn btn-primary w-100" :disabled="loading" type="submit">
            {{ loading ? 'Signing in…' : 'Sign in' }}
          </button>
        </form>

        <div class="d-flex justify-content-between align-items-center gap-3 flex-wrap mt-3">
          <RouterLink class="btn btn-link p-0" to="/login"><i class="bi bi-arrow-left" /> Back</RouterLink>
          <div class="text-secondary small">Theme is stored per role ({{ expectedRole }}).</div>
        </div>
      </div>
    </div>
  </div>
</template>

