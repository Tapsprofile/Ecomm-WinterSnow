import { defineStore } from 'pinia'
import { computed, watch } from 'vue'
import { useAuthStore } from './auth'

const STORAGE_KEY = 'wintersnow.theme'

/**
 * Theme is stored per role (Customer/Vendor/Admin).
 * Default: Customer=light, Vendor=dark, Admin=dark.
 */
export const useThemeStore = defineStore('theme', () => {
  const auth = useAuthStore()

  const saved = safeJsonParse(localStorage.getItem(STORAGE_KEY)) || {}
  const modeByRole = {
    Customer: saved.Customer || 'light',
    Vendor: saved.Vendor || 'dark',
    Admin: saved.Admin || 'dark'
  }

  const roleKey = computed(() => auth.role || 'Customer')
  const mode = computed(() => modeByRole[roleKey.value] || 'light')

  function apply() {
    document.documentElement.dataset.theme = mode.value
    document.documentElement.dataset.role = roleKey.value
  }

  function setModeForRole(role, nextMode) {
    modeByRole[role] = nextMode
    localStorage.setItem(STORAGE_KEY, JSON.stringify(modeByRole))
    apply()
  }

  function toggle() {
    const next = mode.value === 'dark' ? 'light' : 'dark'
    setModeForRole(roleKey.value, next)
  }

  // keep DOM in sync when auth role changes
  watch(roleKey, apply, { immediate: true })

  return { roleKey, mode, apply, toggle, setModeForRole }
})

function safeJsonParse(v) {
  try {
    return JSON.parse(v)
  } catch {
    return null
  }
}

