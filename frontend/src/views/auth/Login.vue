<script setup>
import TopNav from '../../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container py-4">
    <div class="card ws-card mx-auto" style="max-width: 920px;">
      <div class="card-body p-4 p-lg-5">
        <div class="ws-section-kicker mb-2">Choose portal</div>
        <h2 class="ws-section-title mb-1">Login</h2>
        <div class="text-secondary">Separate login screens for Customer, Vendor, and System Admin.</div>

        <div class="row g-3 mt-3">
          <div class="col-12 col-md-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">Customer</div>
                <div class="text-secondary small mt-1">Storefront shopper account.</div>
                <RouterLink class="btn ws-btn-dark w-100 mt-3" to="/login/customer">
                  Customer login
                </RouterLink>
              </div>
            </div>
          </div>
          <div class="col-12 col-md-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">Vendor</div>
                <div class="text-secondary small mt-1">Seller cockpit for listings & inventory.</div>
                <RouterLink class="btn ws-btn-dark w-100 mt-3" to="/login/vendor">
                  Vendor login
                </RouterLink>
              </div>
            </div>
          </div>
          <div class="col-12 col-md-4">
            <div class="card ws-card h-100">
              <div class="card-body">
                <div class="fw-bold">System Admin</div>
                <div class="text-secondary small mt-1">Governance, finance, health, marketing.</div>
                <RouterLink class="btn ws-btn-dark w-100 mt-3" to="/login/admin">
                  Admin login
                </RouterLink>
              </div>
            </div>
          </div>
        </div>

        <div class="card ws-card mt-3">
          <div class="card-body">
            <div class="fw-bold">Demo accounts</div>
            <div class="text-secondary small mt-1">
              Admin: <b>admin@demo.local</b> / Admin123! • Vendor: <b>vendor@demo.local</b> / Vendor123! • Customer: <b>customer@demo.local</b> / Customer123!
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

