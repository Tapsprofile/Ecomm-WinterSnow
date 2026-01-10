<script setup>
import { onMounted, ref } from 'vue'
import { apiGet, apiPost } from '../../lib/api'

const loading = ref(true)
const error = ref(null)
const vendors = ref([])
const products = ref([])

async function load() {
  loading.value = true
  error.value = null
  try {
    vendors.value = await apiGet('/api/admin/moderation/vendors')
    products.value = await apiGet('/api/admin/moderation/products')
  } catch (e) {
    error.value = e?.message || 'Failed to load moderation lists'
  } finally {
    loading.value = false
  }
}

async function approveVendor(vendorId) {
  await apiPost(`/api/admin/moderation/vendors/${vendorId}/approve-kyc`, {})
  await load()
}

async function approveProduct(productId) {
  await apiPost(`/api/admin/moderation/products/${productId}/approve`, {})
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
    <div class="col card">
      <div style="font-weight:900;">Vendor Moderation</div>
      <div class="muted">New registrations requiring approval (KYC).</div>
      <table class="table" style="margin-top:10px;">
        <thead>
          <tr><th>Vendor</th><th>Status</th><th></th></tr>
        </thead>
        <tbody>
          <tr v-for="v in vendors" :key="v.vendorId">
            <td><b>#{{ v.vendorId }}</b> {{ v.name }}</td>
            <td><span class="pill">KYC: {{ v.isKycApproved ? 'Approved' : 'Pending' }}</span></td>
            <td style="text-align:right;">
              <button class="btn primary" :disabled="v.isKycApproved" @click="approveVendor(v.vendorId)">Approve</button>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="!vendors.length" class="muted" style="margin-top:10px;">No pending vendors.</div>
    </div>

    <div class="col card">
      <div style="font-weight:900;">Product Submissions</div>
      <div class="muted">Products requiring approval to publish.</div>
      <table class="table" style="margin-top:10px;">
        <thead>
          <tr><th>Product</th><th>Vendor</th><th></th></tr>
        </thead>
        <tbody>
          <tr v-for="p in products" :key="p.productId">
            <td><b>#{{ p.productId }}</b> {{ p.name }} <span class="muted">/{{ p.slug }}</span></td>
            <td>#{{ p.vendorId }}</td>
            <td style="text-align:right;">
              <button class="btn primary" :disabled="p.isApprovedByAdmin" @click="approveProduct(p.productId)">Approve</button>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="!products.length" class="muted" style="margin-top:10px;">No pending products.</div>
    </div>
  </div>
</template>

