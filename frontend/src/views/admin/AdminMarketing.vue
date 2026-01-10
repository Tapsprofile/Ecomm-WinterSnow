<script setup>
import { onMounted, ref } from 'vue'
import { apiGet, apiPost } from '../../lib/api'

const loading = ref(true)
const error = ref(null)

const banners = ref([])
const coupons = ref([])
const flashSales = ref([])

const newBanner = ref({ title: '', imageUrl: '', targetUrl: '', isActive: true })
const newCoupon = ref({ code: '', description: '', discountAmount: 0, isActive: true })
const newFlash = ref({
  name: '',
  discountPercent: 10,
  isActive: true,
  startUtc: new Date().toISOString(),
  endUtc: new Date(Date.now() + 7 * 24 * 3600 * 1000).toISOString()
})

async function load() {
  loading.value = true
  error.value = null
  try {
    banners.value = await apiGet('/api/admin/marketing/banners')
    coupons.value = await apiGet('/api/admin/marketing/coupons')
    flashSales.value = await apiGet('/api/admin/marketing/flash-sales')
  } catch (e) {
    error.value = e?.message || 'Failed to load marketing data'
  } finally {
    loading.value = false
  }
}

async function createBanner() {
  await apiPost('/api/admin/marketing/banners', newBanner.value)
  newBanner.value = { title: '', imageUrl: '', targetUrl: '', isActive: true }
  await load()
}

async function createCoupon() {
  await apiPost('/api/admin/marketing/coupons', newCoupon.value)
  newCoupon.value = { code: '', description: '', discountAmount: 0, isActive: true }
  await load()
}

async function createFlashSale() {
  await apiPost('/api/admin/marketing/flash-sales', newFlash.value)
  newFlash.value = {
    name: '',
    discountPercent: 10,
    isActive: true,
    startUtc: new Date().toISOString(),
    endUtc: new Date(Date.now() + 7 * 24 * 3600 * 1000).toISOString()
  }
  await load()
}

onMounted(load)
</script>

<template>
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1 align-items-start">
    <div class="col-12 col-lg-6">
      <div class="card">
        <div class="card-body">
          <div class="fw-bold">Sitewide Banners</div>
          <div class="text-secondary small">Create banners for winter sales.</div>

          <div class="mt-3 d-flex flex-column gap-2">
            <input class="form-control" v-model="newBanner.title" placeholder="Title" />
            <input class="form-control" v-model="newBanner.imageUrl" placeholder="Image URL" />
            <input class="form-control" v-model="newBanner.targetUrl" placeholder="Target URL" />
            <div class="form-check">
              <input class="form-check-input" type="checkbox" id="bannerActive" v-model="newBanner.isActive" />
              <label class="form-check-label" for="bannerActive">Active</label>
            </div>
            <button class="btn btn-primary" @click="createBanner">Create banner</button>
          </div>

          <div class="table-responsive mt-3">
            <table class="table align-middle">
              <thead><tr><th>Title</th><th>Active</th></tr></thead>
              <tbody>
                <tr v-for="b in banners" :key="b.id">
                  <td><b>{{ b.title }}</b> <span class="text-secondary small ms-2">{{ b.targetUrl }}</span></td>
                  <td><span class="badge" :class="b.isActive ? 'text-bg-success' : 'text-bg-secondary'">{{ b.isActive ? 'Yes' : 'No' }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div class="col-12 col-lg-6">
      <div class="card">
        <div class="card-body">
          <div class="fw-bold">Discount Codes</div>
          <div class="text-secondary small">Coupons and promotions engine basics.</div>

          <div class="mt-3 d-flex flex-column gap-2">
            <input class="form-control" v-model="newCoupon.code" placeholder="Code (e.g. WINTER10)" />
            <input class="form-control" v-model="newCoupon.description" placeholder="Description" />
            <input class="form-control" type="number" v-model.number="newCoupon.discountAmount" placeholder="Discount amount" />
            <div class="form-check">
              <input class="form-check-input" type="checkbox" id="couponActive" v-model="newCoupon.isActive" />
              <label class="form-check-label" for="couponActive">Active</label>
            </div>
            <button class="btn btn-primary" @click="createCoupon">Create coupon</button>
          </div>

          <div class="table-responsive mt-3">
            <table class="table align-middle">
              <thead><tr><th>Code</th><th>Discount</th></tr></thead>
              <tbody>
                <tr v-for="c in coupons" :key="c.id">
                  <td><b>{{ c.code }}</b> <span class="text-secondary small ms-2">{{ c.description }}</span></td>
                  <td>INR {{ c.discountAmount }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="card mt-3">
        <div class="card-body">
          <div class="fw-bold">Flash Sales</div>
          <div class="text-secondary small">Create timed sales events.</div>

          <div class="mt-3 d-flex flex-column gap-2">
            <input class="form-control" v-model="newFlash.name" placeholder="Event name" />
            <input class="form-control" type="number" v-model.number="newFlash.discountPercent" placeholder="Discount percent" />
            <div class="form-check">
              <input class="form-check-input" type="checkbox" id="flashActive" v-model="newFlash.isActive" />
              <label class="form-check-label" for="flashActive">Active</label>
            </div>
            <button class="btn btn-primary" @click="createFlashSale">Create flash sale</button>
          </div>

          <div class="table-responsive mt-3">
            <table class="table align-middle">
              <thead><tr><th>Name</th><th>Discount</th></tr></thead>
              <tbody>
                <tr v-for="f in flashSales" :key="f.id">
                  <td><b>{{ f.name }}</b> <span class="text-secondary small ms-2">{{ f.startUtc }} → {{ f.endUtc }}</span></td>
                  <td>{{ f.discountPercent }}%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

