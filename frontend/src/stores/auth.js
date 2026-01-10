import { defineStore } from 'pinia'
import { apiPost } from '../lib/api'

const STORAGE_KEY = 'wintersnow.auth'

export const useAuthStore = defineStore('auth', {
  state: () => {
    const saved = safeJsonParse(localStorage.getItem(STORAGE_KEY)) || {}
    return {
      accessToken: saved.accessToken || null,
      role: saved.role || null,
      vendorId: saved.vendorId || null
    }
  },
  getters: {
    isAuthenticated: (s) => !!s.accessToken
  },
  actions: {
    async login(email, password) {
      const res = await apiPost('/api/auth/login', { email, password })
      this.accessToken = res.accessToken
      this.role = res.role
      this.vendorId = res.vendorId ?? null
      localStorage.setItem(STORAGE_KEY, JSON.stringify({ accessToken: this.accessToken, role: this.role, vendorId: this.vendorId }))
      return res
    },
    logout() {
      this.accessToken = null
      this.role = null
      this.vendorId = null
      localStorage.removeItem(STORAGE_KEY)
    }
  }
})

function safeJsonParse(v) {
  try {
    return JSON.parse(v)
  } catch {
    return null
  }
}

