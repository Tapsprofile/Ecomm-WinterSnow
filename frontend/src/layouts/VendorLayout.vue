<script setup>
import TopNav from '../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container py-3">
    <div class="card">
      <div class="card-body">
        <div class="d-flex justify-content-between align-items-start gap-3 flex-wrap">
          <div>
            <span class="badge text-bg-secondary">Vendor Dashboard</span>
            <h2 class="h4 mt-2 mb-0">Seller Cockpit</h2>
          </div>
          <ul class="nav nav-pills">
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

