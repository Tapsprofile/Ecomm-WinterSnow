import { defineStore } from 'pinia'

const STORAGE_KEY = 'wintersnow.cart'

export const useCartStore = defineStore('cart', {
  state: () => {
    const saved = safeJsonParse(localStorage.getItem(STORAGE_KEY)) || {}
    return {
      items: saved.items || []
    }
  },
  getters: {
    totalItems: (s) => s.items.reduce((sum, i) => sum + i.quantity, 0)
  },
  actions: {
    addItem({ productId, variantId, quantity = 1, name }) {
      const existing = this.items.find((i) => i.productId === productId && i.variantId === variantId)
      if (existing) existing.quantity += quantity
      else this.items.push({ productId, variantId: variantId ?? null, quantity, name: name ?? null })
      persist(this.items)
    },
    setQuantity(productId, variantId, quantity) {
      const item = this.items.find((i) => i.productId === productId && i.variantId === variantId)
      if (!item) return
      item.quantity = Math.max(1, quantity)
      persist(this.items)
    },
    removeItem(productId, variantId) {
      this.items = this.items.filter((i) => !(i.productId === productId && i.variantId === variantId))
      persist(this.items)
    },
    clear() {
      this.items = []
      persist(this.items)
    }
  }
})

function persist(items) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify({ items }))
}

function safeJsonParse(v) {
  try {
    return JSON.parse(v)
  } catch {
    return null
  }
}

