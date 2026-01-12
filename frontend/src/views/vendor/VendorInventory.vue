<script setup>
import { computed, onMounted, ref } from 'vue'
import { apiGet, apiPost, apiPut } from '../../lib/api'

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
    for (const p of items.value) {
      p.allowedPostcodesText = (p.allowedPostcodes || []).join(', ')
    }
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
    // Save product-level fields (discounts/coupon eligibility) first.
    for (const p of items.value) {
      const allowedPostcodes =
        typeof p.allowedPostcodesText === 'string'
          ? p.allowedPostcodesText
              .split(/[\n,]+/g)
              .map((x) => x.trim())
              .filter(Boolean)
          : []

      await apiPut(`/api/vendor/listings/${p.productId}`, {
        name: p.name,
        shortDescription: p.shortDescription ?? null,
        fullDescription: p.fullDescription ?? null,
        material: p.material ?? null,
        categoryId: p.categoryId ?? null,
        price: p.price,
        allowCoupons: !!p.allowCoupons,
        isVisibleInStorefront: !!p.isVisibleInStorefront,
        listingStatus: p.listingStatus || 'Draft',
        allowedPostcodes,
        discountPercent: p.discountPercent ?? null,
        discountStartUtc: p.discountStartUtc ?? null,
        discountEndUtc: p.discountEndUtc ?? null
      })
    }

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
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="card ws-card mt-3">
    <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
      <div>
        <div class="ws-section-kicker mb-1">Inventory manager</div>
        <div class="fw-bold">Bulk edit listings</div>
        <div class="text-secondary small">Search, bulk edit prices/stock, manage discounts and coupon eligibility.</div>
      </div>
      <div class="d-flex gap-2 align-items-center flex-wrap">
        <input class="form-control" style="width: 260px;" v-model="q" placeholder="Search products…" />
        <button class="btn ws-btn-dark" type="button" :disabled="saving" @click="bulkSave">{{ saving ? 'Saving…' : 'Bulk Save' }}</button>
      </div>
    </div>
  </div>

  <div v-if="!loading" class="card ws-card mt-3">
    <div class="card-body">
      <div class="table-responsive">
        <table class="table align-middle">
          <thead>
            <tr>
              <th>Product</th>
              <th>Base Price</th>
              <th>Status</th>
              <th style="min-width: 240px;">Lifecycle / Visibility</th>
              <th style="min-width: 260px;">Postcodes</th>
              <th style="min-width: 260px;">Discount / Coupons</th>
              <th style="min-width: 420px;">Variations (Size/Color Matrix)</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in filtered" :key="p.productId">
              <td>
                <div class="fw-semibold">{{ p.name }}</div>
                <div class="text-secondary small">/{{ p.slug }}</div>
              </td>
              <td>INR {{ p.price }}</td>
              <td>
                <span v-if="p.isApprovedByAdmin && p.published" class="badge text-bg-success">Live</span>
                <span v-else class="badge text-bg-secondary">Pending approval</span>
              </td>
              <td>
                <div class="d-flex flex-column gap-2">
                  <div>
                    <label class="form-label small text-secondary mb-1">Listing status</label>
                    <select class="form-select form-select-sm" v-model="p.listingStatus">
                      <option value="Draft">Draft</option>
                      <option value="Active">Active</option>
                      <option value="EndOfLife">End Of Life (EOL)</option>
                    </select>
                  </div>
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" :id="`visible-${p.productId}`" v-model="p.isVisibleInStorefront" />
                    <label class="form-check-label" :for="`visible-${p.productId}`">Visible to customers</label>
                  </div>
                </div>
              </td>
              <td>
                <label class="form-label small text-secondary mb-1">Allowed postcodes (optional)</label>
                <textarea
                  class="form-control form-control-sm"
                  rows="3"
                  v-model="p.allowedPostcodesText"
                  placeholder="e.g. 560001, 110001"
                />
                <div class="text-secondary small mt-1">If set, listing is shown only for those postcodes.</div>
              </td>
              <td>
                <div class="d-flex flex-column gap-2">
                  <div class="form-check">
                    <input class="form-check-input" type="checkbox" :id="`allowCoupons-${p.productId}`" v-model="p.allowCoupons" />
                    <label class="form-check-label" :for="`allowCoupons-${p.productId}`">Allow coupons</label>
                  </div>
                  <div>
                    <label class="form-label small text-secondary mb-1">Discount %</label>
                    <input class="form-control form-control-sm" type="number" min="0" max="100" v-model.number="p.discountPercent" />
                  </div>
                  <div class="row g-2">
                    <div class="col-6">
                      <label class="form-label small text-secondary mb-1">Start (UTC)</label>
                      <input class="form-control form-control-sm" v-model="p.discountStartUtc" placeholder="2026-01-10T00:00:00Z" />
                    </div>
                    <div class="col-6">
                      <label class="form-label small text-secondary mb-1">End (UTC)</label>
                      <input class="form-control form-control-sm" v-model="p.discountEndUtc" placeholder="2026-01-17T00:00:00Z" />
                    </div>
                  </div>
                </div>
              </td>
              <td>
                <div class="d-flex flex-column gap-2">
                  <div v-for="v in p.variants" :key="v.variantId" class="card ws-card">
                    <div class="card-body py-2">
                      <div class="d-flex justify-content-between align-items-center gap-3 flex-wrap">
                        <div>
                          <b>{{ v.size || '-' }} / {{ v.color || '-' }}</b>
                          <div class="text-secondary small">Variant #{{ v.variantId }}</div>
                        </div>
                        <div class="d-flex gap-2 align-items-end flex-wrap">
                          <div>
                            <div class="text-secondary small mb-1">Override price</div>
                            <input class="form-control form-control-sm" style="width: 140px;" type="number" v-model.number="v.overridePrice" />
                          </div>
                          <div>
                            <div class="text-secondary small mb-1">Stock</div>
                            <input class="form-control form-control-sm" style="width: 120px;" type="number" min="0" v-model.number="v.stockQuantity" />
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

