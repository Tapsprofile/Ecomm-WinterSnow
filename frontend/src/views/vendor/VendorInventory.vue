<script setup>
import { computed, onMounted, ref } from 'vue'
import { apiGet, apiPost } from '../../lib/api'

const loading = ref(true)
const saving = ref(false)
const error = ref(null)
const items = ref([])
const q = ref('')

const filtered = computed(() => {
  const term = q.value.trim().toLowerCase()
  if (!term) return items.value
  return items.value.filter((p) => p.name.toLowerCase().includes(term) || p.slug.toLowerCase().includes(term))
})

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    items.value = await apiGet('/api/vendor/inventory')
  } catch (e) {
    error.value = e?.message || 'Failed to load inventory'
  } finally {
    loading.value = false
  }
})

async function bulkSave() {
  saving.value = true
  error.value = null
  try {
    const variants = []
    for (const p of items.value) {
      for (const v of p.variants) {
        variants.push({
          variantId: v.variantId,
          overridePrice: v.overridePrice ?? null,
          stockQuantity: v.stockQuantity
        })
      }
    }
    await apiPost('/api/vendor/inventory/bulk-update', { variants })
  } catch (e) {
    error.value = e?.message || 'Bulk update failed'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <div style="display:flex; justify-content:space-between; gap:10px; align-items:center; flex-wrap:wrap;">
      <div>
        <div style="font-weight:900;">Inventory Manager</div>
        <div class="muted">Search, bulk edit prices/stock, manage variations.</div>
      </div>
      <div style="display:flex; gap:8px; align-items:center; flex-wrap:wrap;">
        <input class="input" style="width:260px;" v-model="q" placeholder="Search products…" />
        <button class="btn primary" :disabled="saving" @click="bulkSave">{{ saving ? 'Saving…' : 'Bulk Save' }}</button>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card" style="margin-top:12px;">
    <table class="table">
      <thead>
        <tr>
          <th>Product</th>
          <th>Base Price</th>
          <th>Status</th>
          <th style="width:45%;">Variations (Size/Color Matrix)</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in filtered" :key="p.productId">
          <td>
            <div style="font-weight:800;">{{ p.name }}</div>
            <div class="muted" style="font-size:12px;">/{{ p.slug }}</div>
          </td>
          <td>INR {{ p.price }}</td>
          <td>
            <span class="pill" v-if="p.isApprovedByAdmin && p.published">Live</span>
            <span class="pill" v-else>Pending approval</span>
          </td>
          <td>
            <div style="display:flex; flex-direction:column; gap:10px;">
              <div v-for="v in p.variants" :key="v.variantId" class="card" style="background:#f8fafc;">
                <div style="display:flex; justify-content:space-between; gap:10px; align-items:center; flex-wrap:wrap;">
                  <div>
                    <b>{{ v.size || '-' }} / {{ v.color || '-' }}</b>
                    <div class="muted" style="font-size:12px;">Variant #{{ v.variantId }}</div>
                  </div>
                  <div style="display:flex; gap:8px; align-items:center; flex-wrap:wrap;">
                    <label style="display:flex; flex-direction:column; gap:4px;">
                      <span class="muted" style="font-size:12px;">Override price</span>
                      <input class="input" style="width:140px;" type="number" v-model.number="v.overridePrice" />
                    </label>
                    <label style="display:flex; flex-direction:column; gap:4px;">
                      <span class="muted" style="font-size:12px;">Stock</span>
                      <input class="input" style="width:120px;" type="number" min="0" v-model.number="v.stockQuantity" />
                    </label>
                  </div>
                </div>
              </div>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

