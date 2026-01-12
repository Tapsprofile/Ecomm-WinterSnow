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
            <div class="ws-section-kicker mb-1">Admin & system panel</div>
            <h2 class="ws-section-title mb-0">Platform Command Center</h2>
          </div>
          <ul class="nav nav-pills ws-pills gap-2">
            <li class="nav-item"><RouterLink class="nav-link" to="/admin">Moderation</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/admin/finance">Finance</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/admin/system">System Health</RouterLink></li>
            <li class="nav-item"><RouterLink class="nav-link" to="/admin/marketing">Marketing</RouterLink></li>
          </ul>
        </div>
      </div>
    </div>
    <router-view />
  </div>
</template>

