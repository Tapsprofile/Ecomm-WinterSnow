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
  <div class="card" style="margin-top:12px;">
    <div style="display:flex; justify-content:space-between; gap:10px; align-items:center; flex-wrap:wrap;">
      <div>
        <div style="font-weight:900;">Create Product Listing</div>
        <div class="muted">Add inventory, pricing, discounts, coupon-eligibility, variants, and media.</div>
      </div>
      <RouterLink class="btn" to="/vendor/inventory">Back to inventory</RouterLink>
    </div>
  </div>

  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="row" style="margin-top:12px; align-items:flex-start;">
    <div class="col card">
      <div style="font-weight:900; margin-bottom:10px;">Listing details</div>
      <label style="display:block; margin-bottom:10px;">
        <div class="muted" style="font-size:12px; margin-bottom:6px;">Name</div>
        <input class="input" v-model="form.name" @blur="form.slug = form.slug || slugify(form.name)" />
      </label>
      <label style="display:block; margin-bottom:10px;">
        <div class="muted" style="font-size:12px; margin-bottom:6px;">Slug</div>
        <input class="input" v-model="form.slug" placeholder="e.g. thermal-gloves" />
      </label>
      <label style="display:block; margin-bottom:10px;">
        <div class="muted" style="font-size:12px; margin-bottom:6px;">Short description</div>
        <input class="input" v-model="form.shortDescription" />
      </label>
      <label style="display:block; margin-bottom:10px;">
        <div class="muted" style="font-size:12px; margin-bottom:6px;">Full description / specs</div>
        <textarea class="textarea" v-model="form.fullDescription" />
      </label>

      <div class="row">
        <label class="col">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Material</div>
          <input class="input" v-model="form.material" placeholder="Fleece, Polyester…" />
        </label>
        <label class="col">
          <div class="muted" style="font-size:12px; margin-bottom:6px;">Category</div>
          <select class="select" v-model="form.categoryId">
            <option :value="null">Uncategorized</option>
            <optgroup v-for="r in rootCats" :key="r.categoryId" :label="r.name">
              <option v-for="c in childCats.filter((x) => x.parentCategoryId === r.categoryId)" :key="c.categoryId" :value="c.categoryId">
                {{ c.name }}
              </option>
            </optgroup>
          </select>
        </label>
      </div>
    </div>

    <div class="col">
      <div class="card">
        <div style="font-weight:900; margin-bottom:10px;">Pricing, discounts, coupons</div>
        <div class="row">
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Base price (INR)</div>
            <input class="input" type="number" min="0" v-model.number="form.price" />
          </label>
          <label class="col" style="display:flex; align-items:flex-end;">
            <label style="display:flex; gap:8px; align-items:center;">
              <input type="checkbox" v-model="form.allowCoupons" />
              Allow coupons
            </label>
          </label>
        </div>

        <div class="row" style="margin-top:10px;">
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Discount %</div>
            <input class="input" type="number" min="0" max="100" v-model.number="form.discountPercent" />
          </label>
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Discount start (UTC)</div>
            <input class="input" v-model="form.discountStartUtc" placeholder="2026-01-10T00:00:00Z" />
          </label>
          <label class="col">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Discount end (UTC)</div>
            <input class="input" v-model="form.discountEndUtc" placeholder="2026-01-17T00:00:00Z" />
          </label>
        </div>
      </div>

      <div class="card" style="margin-top:12px;">
        <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
          <div style="font-weight:900;">Variants (Inventory)</div>
          <button class="btn" type="button" @click="addVariant">Add variant</button>
        </div>
        <div style="display:flex; flex-direction:column; gap:10px; margin-top:10px;">
          <div v-for="(v, idx) in form.variants" :key="idx" class="card" style="background:#f8fafc;">
            <div class="row">
              <label class="col">
                <div class="muted" style="font-size:12px; margin-bottom:6px;">Size</div>
                <input class="input" v-model="v.size" />
              </label>
              <label class="col">
                <div class="muted" style="font-size:12px; margin-bottom:6px;">Color</div>
                <input class="input" v-model="v.color" />
              </label>
              <label class="col">
                <div class="muted" style="font-size:12px; margin-bottom:6px;">SKU</div>
                <input class="input" v-model="v.sku" />
              </label>
            </div>
            <div class="row" style="margin-top:10px;">
              <label class="col">
                <div class="muted" style="font-size:12px; margin-bottom:6px;">Override price (optional)</div>
                <input class="input" type="number" min="0" v-model.number="v.overridePrice" />
              </label>
              <label class="col">
                <div class="muted" style="font-size:12px; margin-bottom:6px;">Stock quantity</div>
                <input class="input" type="number" min="0" v-model.number="v.stockQuantity" />
              </label>
            </div>
          </div>
        </div>
      </div>

      <div class="card" style="margin-top:12px;">
        <div style="display:flex; justify-content:space-between; gap:10px; align-items:center;">
          <div style="font-weight:900;">Media URLs</div>
          <button class="btn" type="button" @click="addMedia">Add media</button>
        </div>
        <div style="display:flex; flex-direction:column; gap:8px; margin-top:10px;">
          <input v-for="(u, idx) in form.mediaUrls" :key="idx" class="input" v-model="form.mediaUrls[idx]" placeholder="https://..." />
        </div>
      </div>

      <button class="btn primary" style="margin-top:12px; width:100%;" :disabled="saving" @click="submit">
        {{ saving ? 'Creating…' : 'Create listing (goes to Admin approval)' }}
      </button>
    </div>
  </div>
</template>

