<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiGet, apiPost } from '../../lib/api'

const router = useRouter()
const loading = ref(true)
const saving = ref(false)
const error = ref(null)

const categories = ref([])
const rootCats = computed(() => categories.value.filter((c) => c.parentCategoryId == null))
const childCats = computed(() => categories.value.filter((c) => c.parentCategoryId != null))

const form = ref({
  name: '',
  slug: '',
  shortDescription: '',
  fullDescription: '',
  material: '',
  categoryId: null,
  price: 0,
  allowCoupons: true,
  discountPercent: null,
  discountStartUtc: null,
  discountEndUtc: null,
  mediaUrls: [''],
  variants: [{ size: 'Default', color: 'Default', sku: '', overridePrice: null, stockQuantity: 10 }]
})

function slugify(v) {
  return v
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)+/g, '')
}

function addVariant() {
  form.value.variants.push({ size: '', color: '', sku: '', overridePrice: null, stockQuantity: 0 })
}

function addMedia() {
  form.value.mediaUrls.push('')
}

async function load() {
  loading.value = true
  error.value = null
  try {
    categories.value = await apiGet('/api/vendor/categories')
  } catch (e) {
    error.value = e?.message || 'Failed to load categories'
  } finally {
    loading.value = false
  }
}

async function submit() {
  saving.value = true
  error.value = null
  try {
    if (!form.value.slug) form.value.slug = slugify(form.value.name)
    const res = await apiPost('/api/vendor/listings', form.value)
    router.push('/vendor/inventory')
    return res
  } catch (e) {
    error.value = e?.message || 'Failed to create listing'
  } finally {
    saving.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="card ws-card mt-3">
    <div class="card-body d-flex justify-content-between align-items-center gap-3 flex-wrap">
      <div>
        <div class="ws-section-kicker mb-1">Create listing</div>
        <div class="fw-bold">Create Product Listing</div>
        <div class="text-secondary small">Add inventory, pricing, discounts, coupon eligibility, variants, and media.</div>
      </div>
      <RouterLink class="btn ws-btn-outline btn-sm" to="/vendor/inventory">Back to inventory</RouterLink>
    </div>
  </div>

  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1 align-items-start">
    <div class="col-12 col-lg-5">
      <div class="card ws-card">
        <div class="card-body">
          <div class="fw-bold mb-3">Listing details</div>

          <div class="mb-3">
            <label class="form-label">Name</label>
            <input class="form-control" v-model="form.name" @blur="form.slug = form.slug || slugify(form.name)" />
          </div>
          <div class="mb-3">
            <label class="form-label">Slug</label>
            <input class="form-control" v-model="form.slug" placeholder="e.g. thermal-gloves" />
          </div>
          <div class="mb-3">
            <label class="form-label">Short description</label>
            <input class="form-control" v-model="form.shortDescription" />
          </div>
          <div class="mb-3">
            <label class="form-label">Full description / specs</label>
            <textarea class="form-control" rows="5" v-model="form.fullDescription" />
          </div>

          <div class="row g-2">
            <div class="col-6">
              <label class="form-label">Material</label>
              <input class="form-control" v-model="form.material" placeholder="Fleece, Polyester…" />
            </div>
            <div class="col-6">
              <label class="form-label">Category</label>
              <select class="form-select" v-model="form.categoryId">
                <option :value="null">Uncategorized</option>
                <optgroup v-for="r in rootCats" :key="r.categoryId" :label="r.name">
                  <option v-for="c in childCats.filter((x) => x.parentCategoryId === r.categoryId)" :key="c.categoryId" :value="c.categoryId">
                    {{ c.name }}
                  </option>
                </optgroup>
              </select>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="col-12 col-lg-7">
      <div class="card ws-card">
        <div class="card-body">
          <div class="fw-bold mb-3">Pricing, discounts, coupons</div>
          <div class="row g-2 align-items-end">
            <div class="col-6">
              <label class="form-label">Base price (INR)</label>
              <input class="form-control" type="number" min="0" v-model.number="form.price" />
            </div>
            <div class="col-6">
              <div class="form-check mt-4">
                <input class="form-check-input" type="checkbox" id="allowCoupons" v-model="form.allowCoupons" />
                <label class="form-check-label" for="allowCoupons">Allow coupons</label>
              </div>
            </div>
          </div>

          <div class="row g-2 mt-2">
            <div class="col-4">
              <label class="form-label">Discount %</label>
              <input class="form-control" type="number" min="0" max="100" v-model.number="form.discountPercent" />
            </div>
            <div class="col-4">
              <label class="form-label">Discount start (UTC)</label>
              <input class="form-control" v-model="form.discountStartUtc" placeholder="2026-01-10T00:00:00Z" />
            </div>
            <div class="col-4">
              <label class="form-label">Discount end (UTC)</label>
              <input class="form-control" v-model="form.discountEndUtc" placeholder="2026-01-17T00:00:00Z" />
            </div>
          </div>
        </div>
      </div>

      <div class="card ws-card mt-3">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center">
            <div class="fw-bold">Variants (Inventory)</div>
            <button class="btn ws-btn-outline btn-sm" type="button" @click="addVariant">Add variant</button>
          </div>

          <div class="d-flex flex-column gap-2 mt-3">
            <div v-for="(v, idx) in form.variants" :key="idx" class="card ws-card">
              <div class="card-body">
                <div class="row g-2">
                  <div class="col-4">
                    <label class="form-label small text-secondary">Size</label>
                    <input class="form-control form-control-sm" v-model="v.size" />
                  </div>
                  <div class="col-4">
                    <label class="form-label small text-secondary">Color</label>
                    <input class="form-control form-control-sm" v-model="v.color" />
                  </div>
                  <div class="col-4">
                    <label class="form-label small text-secondary">SKU</label>
                    <input class="form-control form-control-sm" v-model="v.sku" />
                  </div>
                </div>
                <div class="row g-2 mt-2">
                  <div class="col-6">
                    <label class="form-label small text-secondary">Override price (optional)</label>
                    <input class="form-control form-control-sm" type="number" min="0" v-model.number="v.overridePrice" />
                  </div>
                  <div class="col-6">
                    <label class="form-label small text-secondary">Stock quantity</label>
                    <input class="form-control form-control-sm" type="number" min="0" v-model.number="v.stockQuantity" />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card ws-card mt-3">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center">
            <div class="fw-bold">Media URLs</div>
            <button class="btn ws-btn-outline btn-sm" type="button" @click="addMedia">Add media</button>
          </div>
          <div class="d-flex flex-column gap-2 mt-3">
            <input v-for="(u, idx) in form.mediaUrls" :key="idx" class="form-control" v-model="form.mediaUrls[idx]" placeholder="https://..." />
          </div>
        </div>
      </div>

      <button class="btn ws-btn-dark w-100 mt-3" type="button" :disabled="saving" @click="submit">
        {{ saving ? 'Creating…' : 'Create listing (goes to Admin approval)' }}
      </button>
    </div>
  </div>
</template>

