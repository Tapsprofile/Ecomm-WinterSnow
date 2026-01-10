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
  <div v-if="error" class="alert alert-danger mt-3" role="alert">
    <b>Error:</b> {{ error }}
  </div>
  <div v-if="loading" class="text-secondary mt-3">Loading…</div>

  <div v-if="!loading" class="row g-3 mt-1 align-items-start">
    <div class="col-12 col-lg-6">
      <div class="card">
        <div class="card-body">
          <div class="fw-bold">Vendor Moderation</div>
          <div class="text-secondary small">New registrations requiring approval (KYC).</div>

          <div class="table-responsive mt-3">
            <table class="table align-middle">
              <thead>
                <tr><th>Vendor</th><th>Status</th><th class="text-end"></th></tr>
              </thead>
              <tbody>
                <tr v-for="v in vendors" :key="v.vendorId">
                  <td><b>#{{ v.vendorId }}</b> {{ v.name }}</td>
                  <td>
                    <span class="badge" :class="v.isKycApproved ? 'text-bg-success' : 'text-bg-secondary'">
                      KYC: {{ v.isKycApproved ? 'Approved' : 'Pending' }}
                    </span>
                  </td>
                  <td class="text-end">
                    <button class="btn btn-primary btn-sm" :disabled="v.isKycApproved" @click="approveVendor(v.vendorId)">Approve</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-if="!vendors.length" class="text-secondary">No pending vendors.</div>
        </div>
      </div>
    </div>

    <div class="col-12 col-lg-6">
      <div class="card">
        <div class="card-body">
          <div class="fw-bold">Product Submissions</div>
          <div class="text-secondary small">Products requiring approval to publish.</div>

          <div class="table-responsive mt-3">
            <table class="table align-middle">
              <thead>
                <tr><th>Product</th><th>Vendor</th><th class="text-end"></th></tr>
              </thead>
              <tbody>
                <tr v-for="p in products" :key="p.productId">
                  <td><b>#{{ p.productId }}</b> {{ p.name }} <span class="text-secondary small">/{{ p.slug }}</span></td>
                  <td>#{{ p.vendorId }}</td>
                  <td class="text-end">
                    <button class="btn btn-primary btn-sm" :disabled="p.isApprovedByAdmin" @click="approveProduct(p.productId)">Approve</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-if="!products.length" class="text-secondary">No pending products.</div>
        </div>
      </div>
    </div>
  </div>
</template>

