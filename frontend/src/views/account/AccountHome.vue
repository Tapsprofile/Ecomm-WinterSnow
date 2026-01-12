<script setup>
import TopNav from '../../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container py-4">
    <div class="card ws-card">
      <div class="card-body p-4">
        <div class="ws-section-kicker mb-1">My account</div>
        <h2 class="ws-section-title mb-1">Customer Portal</h2>
        <div class="text-secondary">WooCommerce-style essentials: orders, returns, and reviews.</div>

        <div class="row g-3 mt-3">
          <div class="col-12 col-md-6 col-lg-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">Orders</div>
                <div class="text-secondary small mt-1">View your order history and order details.</div>
                <RouterLink class="btn ws-btn-dark w-100 mt-3" to="/account/orders">View orders</RouterLink>
              </div>
            </div>
          </div>

          <div class="col-12 col-md-6 col-lg-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">Returns</div>
                <div class="text-secondary small mt-1">Create and track return requests.</div>
                <RouterLink class="btn ws-btn-dark w-100 mt-3" to="/account/returns">View returns</RouterLink>
              </div>
            </div>
          </div>

          <div class="col-12 col-md-6 col-lg-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">Shop</div>
                <div class="text-secondary small mt-1">Continue browsing products.</div>
                <RouterLink class="btn ws-btn-outline w-100 mt-3" to="/store">Go to store</RouterLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

