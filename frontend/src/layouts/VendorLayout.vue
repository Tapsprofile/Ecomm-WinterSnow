<script setup>
import TopNav from '../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container py-3">
    <div class="card ws-card">
      <div class="card-body p-4">
        <div class="d-flex justify-content-between align-items-start gap-3 flex-wrap">
          <div>
            <div class="ws-section-kicker mb-1">Vendor dashboard</div>
            <h2 class="ws-section-title mb-0">Seller Cockpit</h2>
          </div>
          <ul class="nav nav-pills ws-pills gap-2">
            <li class="nav-item"><RouterLink class="nav-link" to="/vendor">Overview</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/vendor/listings/new">Add Listing</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/vendor/inventory">Inventory</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/vendor/orders">Orders</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/vendor/payouts">Payouts</RouterLink></li>
          </ul>
        </div>
      </div>
    </div>

    <router-view />
  </div>
</template>

