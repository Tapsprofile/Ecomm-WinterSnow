<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import TopNav from '../../components/ui/TopNav.vue'
import { apiGet } from '../../lib/api'
import { useCartStore } from '../../stores/cart'

const route = useRoute()
const cart = useCartStore()

const loading = ref(true)
const error = ref(null)
const product = ref(null)
const selectedVariantId = ref(null)
const zoomUrl = ref(null)

const effectiveVariant = computed(() => {
  if (!product.value) return null
  return product.value.variants.find((v) => v.variantId === selectedVariantId.value) || null
})

const effectivePrice = computed(() => effectiveVariant.value?.effectivePrice ?? product.value?.price ?? 0)

function addToCart() {
  if (!product.value) return
  cart.addItem({
    productId: product.value.productId,
    variantId: selectedVariantId.value,
    quantity: 1,
    name: product.value.name
  })
}

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    const slug = route.params.slug.toString()
    product.value = await apiGet(`/api/storefront/products/${encodeURIComponent(slug)}`)
    selectedVariantId.value = product.value.variants?.[0]?.variantId ?? null
  } catch (e) {
    error.value = e?.message || 'Failed to load product'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <TopNav />
  <div class="container">
    <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

    <div v-if="!loading && product" class="row" style="margin-top:16px; align-items:flex-start;">
      <div class="col">
        <div class="card">
          <div style="aspect-ratio: 4 / 3; border-radius:12px; overflow:hidden; border:1px solid #e2e8f0; background:#f1f5f9;">
            <img
              v-if="product.media?.length"
              :src="product.media[0].url"
              alt=""
              style="width:100%; height:100%; object-fit:cover; cursor: zoom-in;"
              @click="zoomUrl = product.media[0].url"
            />
            <div v-else class="muted" style="padding:12px;">No media</div>
          </div>
          <div style="display:flex; gap:8px; margin-top:10px; overflow:auto;">
            <button
              v-for="m in product.media"
              :key="m.url"
              class="btn"
              style="padding:0; width:92px; height:64px; overflow:hidden; border-radius:10px;"
              @click="zoomUrl = m.url"
            >
              <img :src="m.url" alt="" style="width:100%; height:100%; object-fit:cover;" />
            </button>
          </div>
        </div>
      </div>

      <div class="col">
        <div class="card">
          <div class="pill">Product</div>
          <h2 style="margin:10px 0 6px;">{{ product.name }}</h2>
          <div class="muted" v-if="product.shortDescription">{{ product.shortDescription }}</div>

          <div style="margin-top:12px; display:flex; gap:10px; align-items:center; flex-wrap:wrap;">
            <div style="font-weight:900; font-size:22px;">{{ product.currency }} {{ effectivePrice }}</div>
            <span v-if="product.material" class="pill">Material: {{ product.material }}</span>
          </div>

          <div style="margin-top:12px;">
            <div class="muted" style="font-size:12px; margin-bottom:6px;">Variant</div>
            <select class="select" v-model="selectedVariantId">
              <option v-for="v in product.variants" :key="v.variantId" :value="v.variantId">
                {{ v.size || '-' }} / {{ v.color || '-' }} • Stock {{ v.stockQuantity }}
              </option>
            </select>
          </div>

          <div style="margin-top:12px; display:flex; gap:10px;">
            <button class="btn primary" @click="addToCart">Add to Cart</button>
            <RouterLink class="btn" to="/checkout">Go to Checkout</RouterLink>
          </div>

          <div v-if="product.fullDescription" style="margin-top:14px;">
            <div style="font-weight:800; margin-bottom:6px;">Specifications</div>
            <div class="muted" style="white-space:pre-wrap;">{{ product.fullDescription }}</div>
          </div>
        </div>

        <div class="card" style="margin-top:12px;">
          <div style="display:flex; justify-content:space-between; align-items:center;">
            <div style="font-weight:900;">Reviews & UGC</div>
            <span class="pill">{{ product.reviews.length }} reviews</span>
          </div>

          <div v-if="!product.reviews.length" class="muted" style="margin-top:10px;">No reviews yet.</div>

          <div v-for="r in product.reviews" :key="r.reviewId" class="card" style="margin-top:10px; background:#f8fafc;">
            <div style="display:flex; gap:8px; align-items:center; flex-wrap:wrap;">
              <b>{{ r.title }}</b>
              <span class="pill">Rating: {{ r.rating }}/5</span>
              <span v-if="r.isVerifiedPurchase" class="pill">Verified purchase</span>
              <span class="muted" style="font-size:12px;">{{ new Date(r.createdOnUtc).toLocaleString() }}</span>
            </div>
            <div style="margin-top:8px;">{{ r.reviewText }}</div>
            <div v-if="r.mediaUrls?.length" style="display:flex; gap:8px; margin-top:10px; overflow:auto;">
              <img
                v-for="u in r.mediaUrls"
                :key="u"
                :src="u"
                alt=""
                style="width:120px; height:90px; object-fit:cover; border-radius:10px; border:1px solid #e2e8f0; cursor: zoom-in;"
                @click="zoomUrl = u"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="zoomUrl" @click="zoomUrl = null" style="position:fixed; inset:0; background:rgba(2,6,23,.72); display:flex; align-items:center; justify-content:center; padding:20px; z-index:100;">
      <img :src="zoomUrl" alt="" style="max-width:95vw; max-height:85vh; border-radius:12px; background:#fff;" />
    </div>
  </div>
</template>

