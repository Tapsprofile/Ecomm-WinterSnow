<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import TopNav from '../../components/ui/TopNav.vue'
import { apiGet, apiPost } from '../../lib/api'
import { useCartStore } from '../../stores/cart'
import { useAuthStore } from '../../stores/auth'

const route = useRoute()
const cart = useCartStore()
const auth = useAuthStore()

const loading = ref(true)
const error = ref(null)
const product = ref(null)
const selectedVariantId = ref(null)
const zoomUrl = ref(null)

const reviewForm = ref({
  rating: 5,
  title: '',
  reviewText: '',
  mediaUrls: ['']
})
const reviewSaving = ref(false)

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

function addReviewMedia() {
  reviewForm.value.mediaUrls.push('')
}

async function submitReview() {
  if (!product.value) return
  error.value = null
  reviewSaving.value = true
  try {
    await apiPost('/api/reviews', {
      productId: product.value.productId,
      rating: reviewForm.value.rating,
      title: reviewForm.value.title,
      reviewText: reviewForm.value.reviewText,
      mediaUrls: reviewForm.value.mediaUrls
    })
    reviewForm.value = { rating: 5, title: '', reviewText: '', mediaUrls: [''] }
    const slug = route.params.slug.toString()
    product.value = await apiGet(`/api/storefront/products/${encodeURIComponent(slug)}`)
  } catch (e) {
    error.value = e?.message || 'Failed to submit review'
  } finally {
    reviewSaving.value = false
  }
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
  <div class="container py-3">
    <div v-if="error" class="alert alert-danger" role="alert">
      <b>Error:</b> {{ error }}
    </div>
    <div v-if="loading" class="text-secondary">Loading…</div>

    <div v-if="!loading && product" class="row g-3 align-items-start">
      <div class="col-12 col-lg-6">
        <div class="card ws-card">
          <div class="card-body">
            <div class="ratio ratio-4x3 bg-body-tertiary rounded-3 overflow-hidden">
              <img
                v-if="product.media?.length"
                :src="product.media[0].url"
                alt=""
                class="w-100 h-100 ws-cover"
                style="cursor: zoom-in;"
                @click="zoomUrl = product.media[0].url"
              />
              <div v-else class="d-flex align-items-center justify-content-center text-secondary small">No media</div>
            </div>

            <div class="d-flex gap-2 mt-3 overflow-auto">
              <button
                v-for="m in product.media"
                :key="m.url"
                type="button"
                class="btn ws-btn-outline btn-sm p-0"
                style="width: 96px; height: 64px;"
                @click="zoomUrl = m.url"
              >
                <img :src="m.url" alt="" class="w-100 h-100 ws-cover rounded-2" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="col-12 col-lg-6">
        <div class="card ws-card">
          <div class="card-body">
            <div class="ws-section-kicker mb-2">Product</div>
            <h2 class="ws-section-title mb-1">{{ product.name }}</h2>
            <div v-if="product.shortDescription" class="text-secondary">{{ product.shortDescription }}</div>

            <div class="mt-3 d-flex flex-wrap align-items-center gap-2">
              <div class="h5 mb-0 fw-bold">{{ product.currency }} {{ effectivePrice }}</div>
              <span v-if="product.originalPrice" class="text-secondary text-decoration-line-through">
                {{ product.currency }} {{ product.originalPrice }}
              </span>
              <span v-if="product.discountPercent" class="badge text-bg-success">-{{ product.discountPercent }}%</span>
              <span v-if="product.allowCoupons" class="badge text-bg-secondary">Coupons eligible</span>
              <span v-if="product.material" class="badge text-bg-light text-dark">Material: {{ product.material }}</span>
            </div>

            <div class="mt-3">
              <label class="form-label small text-secondary">Variant</label>
              <select class="form-select" v-model="selectedVariantId">
                <option v-for="v in product.variants" :key="v.variantId" :value="v.variantId">
                  {{ v.size || '-' }} / {{ v.color || '-' }} • Stock {{ v.stockQuantity }}
                </option>
              </select>
            </div>

            <div class="d-flex gap-2 mt-3">
              <button type="button" class="btn ws-btn-dark" @click="addToCart">Add to Cart</button>
              <RouterLink class="btn ws-btn-outline" to="/checkout">Checkout</RouterLink>
            </div>

            <div v-if="product.fullDescription" class="mt-4">
              <div class="fw-bold mb-2">Specifications</div>
              <div class="text-secondary" style="white-space: pre-wrap;">{{ product.fullDescription }}</div>
            </div>
          </div>
        </div>

        <div class="card ws-card mt-3">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div class="fw-bold">Reviews & UGC</div>
              <span class="badge text-bg-secondary">{{ product.reviews.length }} reviews</span>
            </div>

            <div v-if="auth.isAuthenticated && auth.role === 'Customer'" class="card ws-card mt-3">
              <div class="card-body">
                <div class="fw-bold">Write a review</div>
                <div class="text-secondary small">WooCommerce-style review submission (verified purchase is detected automatically).</div>

                <div class="row g-2 mt-2">
                  <div class="col-4">
                    <label class="form-label small text-secondary">Rating</label>
                    <select class="form-select form-select-sm" v-model.number="reviewForm.rating">
                      <option :value="5">5</option>
                      <option :value="4">4</option>
                      <option :value="3">3</option>
                      <option :value="2">2</option>
                      <option :value="1">1</option>
                    </select>
                  </div>
                  <div class="col-8">
                    <label class="form-label small text-secondary">Title</label>
                    <input class="form-control form-control-sm" v-model="reviewForm.title" placeholder="Great quality" />
                  </div>
                </div>

                <div class="mt-2">
                  <label class="form-label small text-secondary">Review</label>
                  <textarea class="form-control form-control-sm" rows="3" v-model="reviewForm.reviewText" placeholder="Share details about fit, warmth, delivery, etc." />
                </div>

                <div class="mt-2">
                  <div class="d-flex justify-content-between align-items-center">
                    <label class="form-label small text-secondary mb-0">Media URLs (optional)</label>
                    <button class="btn ws-btn-outline btn-sm" type="button" @click="addReviewMedia">Add</button>
                  </div>
                  <div class="d-flex flex-column gap-2 mt-2">
                    <input
                      v-for="(_, idx) in reviewForm.mediaUrls"
                      :key="idx"
                      class="form-control form-control-sm"
                      v-model="reviewForm.mediaUrls[idx]"
                      placeholder="https://..."
                    />
                  </div>
                </div>

                <button
                  class="btn ws-btn-dark w-100 mt-3"
                  type="button"
                  :disabled="reviewSaving || !reviewForm.title || !reviewForm.reviewText"
                  @click="submitReview"
                >
                  {{ reviewSaving ? 'Submitting…' : 'Submit review' }}
                </button>
              </div>
            </div>

            <div v-if="!product.reviews.length" class="text-secondary mt-3">No reviews yet.</div>

            <div v-for="r in product.reviews" :key="r.reviewId" class="card ws-card mt-3">
              <div class="card-body">
                <div class="d-flex gap-2 align-items-center flex-wrap">
                  <b>{{ r.title }}</b>
                  <span class="badge text-bg-secondary">Rating: {{ r.rating }}/5</span>
                  <span v-if="r.isVerifiedPurchase" class="badge text-bg-success">Verified purchase</span>
                  <span class="text-secondary small">{{ new Date(r.createdOnUtc).toLocaleString() }}</span>
                </div>
                <div class="mt-2">{{ r.reviewText }}</div>
                <div v-if="r.mediaUrls?.length" class="d-flex gap-2 mt-3 overflow-auto">
                  <img
                    v-for="u in r.mediaUrls"
                    :key="u"
                    :src="u"
                    alt=""
                    class="rounded-3 border ws-cover"
                    style="width: 120px; height: 90px; cursor: zoom-in;"
                    @click="zoomUrl = u"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div
      v-if="zoomUrl"
      class="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center p-3"
      style="background: rgba(0,0,0,.72); z-index: 2000;"
      @click="zoomUrl = null"
    >
      <img :src="zoomUrl" alt="" class="rounded-4" style="max-width: 95vw; max-height: 85vh; background: white;" />
    </div>
  </div>
</template>

