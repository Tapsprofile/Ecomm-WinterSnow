<script setup>
import TopNav from '../../components/ui/TopNav.vue'
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

const email = ref('customer@demo.local')
const password = ref('Customer123!')
const error = ref(null)
const loading = ref(false)

async function submit() {
  error.value = null
  loading.value = true
  try {
    await auth.login(email.value, password.value)
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
      <h2 style="margin:0 0 12px;">Login</h2>
      <div class="muted" style="margin-bottom:16px; font-size:14px;">
        Demo accounts: <b>admin@demo.local</b> / Admin123!, <b>vendor@demo.local</b> / Vendor123!, <b>customer@demo.local</b> / Customer123!
      </div>

      <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-bottom:12px;">
        <b>Error:</b> {{ error }}
      </div>

      <form @submit.prevent="submit" style="display:flex; flex-direction:column; gap:10px;">
        <label>
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Email</div>
          <input class="input" v-model="email" />
        </label>
        <label>
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Password</div>
          <input class="input" type="password" v-model="password" />
        </label>
        <button class="btn primary" :disabled="loading" type="submit">{{ loading ? 'Signing in…' : 'Sign in' }}</button>
      </form>
    </div>
  </div>
</template>

