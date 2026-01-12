import { defineStore } from 'pinia'
import { apiGet, apiPost } from '../lib/api'
import { useAuthStore } from './auth'

export const useNotificationsStore = defineStore('notifications', {
  state: () => ({
    unread: 0,
    items: [],
    lastLoadedAt: null
  }),
  actions: {
    async refresh() {
      const auth = useAuthStore()
      if (!auth.isAuthenticated) {
        this.unread = 0
        this.items = []
        return
      }

      const [count, list] = await Promise.all([apiGet('/api/notifications/unread-count'), apiGet('/api/notifications?take=20')])
      this.unread = count.unread || 0
      this.items = Array.isArray(list) ? list : []
      this.lastLoadedAt = Date.now()
    },
    async markRead(id) {
      await apiPost(`/api/notifications/${id}/read`, {})
      await this.refresh()
    },
    async markAllRead() {
      await apiPost('/api/notifications/read-all', {})
      await this.refresh()
    }
  }
})

