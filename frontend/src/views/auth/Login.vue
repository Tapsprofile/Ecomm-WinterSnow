<template>
  <div class="container">
    <div class="card" style="max-width:760px; margin: 24px auto;">
      <div class="pill">Choose portal</div>
      <h2 style="margin:10px 0 6px;">Login</h2>
      <div class="muted">Separate login screens for Customer, Vendor, and System Admin.</div>

      <div class="row" style="margin-top:14px;">
        <div class="col card" style="background: color-mix(in srgb, var(--panel) 85%, var(--bg));">
          <div style="font-weight:900;">Customer</div>
          <div class="muted" style="margin-top:6px;">Storefront shopper account.</div>
          <RouterLink class="btn primary" style="margin-top:10px; display:inline-block;" to="/login/customer">Customer login</RouterLink>
        </div>
        <div class="col card" style="background: color-mix(in srgb, var(--panel) 85%, var(--bg));">
          <div style="font-weight:900;">Vendor</div>
          <div class="muted" style="margin-top:6px;">Seller cockpit for listings & inventory.</div>
          <RouterLink class="btn primary" style="margin-top:10px; display:inline-block;" to="/login/vendor">Vendor login</RouterLink>
        </div>
        <div class="col card" style="background: color-mix(in srgb, var(--panel) 85%, var(--bg));">
          <div style="font-weight:900;">System Admin</div>
          <div class="muted" style="margin-top:6px;">Governance, finance, health, marketing.</div>
          <RouterLink class="btn primary" style="margin-top:10px; display:inline-block;" to="/login/admin">Admin login</RouterLink>
        </div>
      </div>

      <div class="card" style="margin-top:14px;">
        <div style="font-weight:900;">Demo accounts</div>
        <div class="muted" style="margin-top:6px;">
          Admin: <b>admin@demo.local</b> / Admin123! • Vendor: <b>vendor@demo.local</b> / Vendor123! • Customer: <b>customer@demo.local</b> / Customer123!
        </div>
      </div>
    </div>
  </div>
</template>

