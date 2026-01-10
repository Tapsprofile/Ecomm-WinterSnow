<script setup>
import TopNav from '../components/ui/TopNav.vue'
</script>

<template>
  <TopNav />
  <div class="container">
    <div class="card" style="margin-top:16px;">
      <div style="display:flex; justify-content:space-between; align-items:center;">
        <div>
          <div class="pill">Admin & System Panel</div>
          <h2 style="margin:8px 0 0;">Platform Command Center</h2>
        </div>
        <div style="display:flex; gap:8px; flex-wrap:wrap;">
          <RouterLink class="btn" to="/admin">Moderation</RouterLink>
          <RouterLink class="btn" to="/admin/finance">Finance</RouterLink>
          <RouterLink class="btn" to="/admin/system">System Health</RouterLink>
          <RouterLink class="btn" to="/admin/marketing">Marketing</RouterLink>
        </div>
      </div>
    </div>
    <router-view />
  </div>
</template>

