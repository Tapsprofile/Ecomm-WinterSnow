<script setup>
const categories = [
  { label: 'Jackets', icon: 'bi bi-snow', to: '/search?q=jacket' },
  { label: 'Gloves', icon: 'bi bi-hand-index-thumb', to: '/search?q=gloves' },
  { label: 'Boots', icon: 'bi bi-bootstrap', to: '/search?q=boots' },
  { label: 'Accessories', icon: 'bi bi-bag', to: '/search?q=accessories' },
  { label: 'Deals', icon: 'bi bi-tag', to: '/search' }
]
</script>

<template>
  <div class="d-flex gap-2 overflow-auto py-2">
    <RouterLink
      v-for="c in categories"
      :key="c.label"
      class="btn btn-outline-secondary btn-sm flex-shrink-0 d-flex align-items-center gap-2"
      :to="c.to"
    >
      <i :class="c.icon" />
      {{ c.label }}
    </RouterLink>
  </div>
</template>

