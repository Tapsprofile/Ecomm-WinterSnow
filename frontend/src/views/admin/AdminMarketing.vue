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
  <div v-if="error" class="card" style="border-color:#fecaca; background:#fef2f2; margin-top:12px;">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="muted" style="margin-top:12px;">Loading…</div>

  <div v-if="!loading" class="row" style="margin-top:12px; align-items:flex-start;">
    <div class="col">
      <div class="card">
        <div style="font-weight:900;">Sitewide Banners</div>
        <div class="muted">Create banners for winter sales.</div>
        <div style="display:flex; flex-direction:column; gap:10px; margin-top:10px;">
          <input class="input" v-model="newBanner.title" placeholder="Title" />
          <input class="input" v-model="newBanner.imageUrl" placeholder="Image URL" />
          <input class="input" v-model="newBanner.targetUrl" placeholder="Target URL" />
          <label style="display:flex; gap:8px; align-items:center;">
            <input type="checkbox" v-model="newBanner.isActive" /> Active
          </label>
          <button class="btn primary" @click="createBanner">Create banner</button>
        </div>

        <table class="table" style="margin-top:12px;">
          <thead><tr><th>Title</th><th>Active</th></tr></thead>
          <tbody>
            <tr v-for="b in banners" :key="b.id">
              <td><b>{{ b.title }}</b> <span class="muted">{{ b.targetUrl }}</span></td>
              <td><span class="pill">{{ b.isActive ? 'Yes' : 'No' }}</span></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="col">
      <div class="card">
        <div style="font-weight:900;">Discount Codes</div>
        <div class="muted">Coupons and promotions engine basics.</div>

        <div style="display:flex; flex-direction:column; gap:10px; margin-top:10px;">
          <input class="input" v-model="newCoupon.code" placeholder="Code (e.g. WINTER10)" />
          <input class="input" v-model="newCoupon.description" placeholder="Description" />
          <input class="input" type="number" v-model.number="newCoupon.discountAmount" placeholder="Discount amount" />
          <label style="display:flex; gap:8px; align-items:center;">
            <input type="checkbox" v-model="newCoupon.isActive" /> Active
          </label>
          <button class="btn primary" @click="createCoupon">Create coupon</button>
        </div>

        <table class="table" style="margin-top:12px;">
          <thead><tr><th>Code</th><th>Discount</th></tr></thead>
          <tbody>
            <tr v-for="c in coupons" :key="c.id">
              <td><b>{{ c.code }}</b> <span class="muted">{{ c.description }}</span></td>
              <td>INR {{ c.discountAmount }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="card" style="margin-top:12px;">
        <div style="font-weight:900;">Flash Sales</div>
        <div class="muted">Create timed sales events.</div>

        <div style="display:flex; flex-direction:column; gap:10px; margin-top:10px;">
          <input class="input" v-model="newFlash.name" placeholder="Event name" />
          <input class="input" type="number" v-model.number="newFlash.discountPercent" placeholder="Discount percent" />
          <label style="display:flex; gap:8px; align-items:center;">
            <input type="checkbox" v-model="newFlash.isActive" /> Active
          </label>
          <button class="btn primary" @click="createFlashSale">Create flash sale</button>
        </div>

        <table class="table" style="margin-top:12px;">
          <thead><tr><th>Name</th><th>Discount</th></tr></thead>
          <tbody>
            <tr v-for="f in flashSales" :key="f.id">
              <td><b>{{ f.name }}</b> <span class="muted">{{ f.startUtc }} → {{ f.endUtc }}</span></td>
              <td>{{ f.discountPercent }}%</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

