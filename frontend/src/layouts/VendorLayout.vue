<script setup>
import TopNav from '../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container">
    <div class="card" style="margin-top:16px;">
      <div style="display:flex; justify-content:space-between; align-items:center;">
        <div>
          <div class="pill">Vendor Dashboard</div>
          <h2 style="margin:8px 0 0;">Seller Cockpit</h2>
        </div>
        <div style="display:flex; gap:8px; flex-wrap:wrap;">
          <RouterLink class="btn" to="/vendor">Overview</RouterLink>
          <RouterLink class="btn primary" to="/vendor/listings/new">Add Listing</RouterLink>
          <RouterLink class="btn" to="/vendor/inventory">Inventory</RouterLink>
          <RouterLink class="btn" to="/vendor/orders">Orders</RouterLink>
          <RouterLink class="btn" to="/vendor/payouts">Payouts</RouterLink>
        </div>
      </div>
    </div>
    <router-view />
  </div>
</template>

