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
            <span class="badge text-bg-secondary">Admin & System Panel</span>
            <h2 class="h4 mt-2 mb-0">Platform Command Center</h2>
          </div>
          <ul class="nav nav-pills">
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

