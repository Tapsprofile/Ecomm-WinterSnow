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
  <div class="container">
    <div class="card" style="max-width:520px; margin: 24px auto;">
      <div class="pill">{{ expectedRole }} portal</div>
      <h2 style="margin:10px 0 6px;">{{ title() }}</h2>
      <div class="muted">Use the credentials for the selected portal.</div>

      <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
        <b>Error:</b> {{ error }}
      </div>

      <form @submit.prevent="submit" style="display:flex; flex-direction:column; gap:10px; margin-top:12px;">
        <label>
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Email</div>
          <input class="input" v-model="email" autocomplete="username" />
        </label>
        <label>
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Password</div>
          <input class="input" type="password" v-model="password" autocomplete="current-password" />
        </label>
        <button class="btn primary" :disabled="loading" type="submit">{{ loading ? 'Signing in…' : 'Sign in' }}</button>
      </form>

      <div style="margin-top:12px; display:flex; justify-content:space-between; gap:10px; flex-wrap:wrap;">
        <RouterLink class="btn" to="/login">Back</RouterLink>
        <div class="muted" style="font-size:12px;">
          Theme is stored per role ({{ expectedRole }}).
        </div>
      </div>
    </div>
  </div>
</template>

