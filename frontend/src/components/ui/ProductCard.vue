<script setup>
defineProps({
  item: { type: Object, required: true }
})
</script>

<template>
  <RouterLink class="card ws-card h-100 text-decoration-none" :to="`/p/${item.slug}`">
    <div class="ratio ratio-4x3 bg-body-tertiary rounded-3 overflow-hidden">
      <img v-if="item.thumbnailUrl" :src="item.thumbnailUrl" alt="" class="w-100 h-100 ws-cover" loading="lazy" />
      <div v-else class="d-flex align-items-center justify-content-center text-secondary small">No image</div>
    </div>

    <div class="mt-3 fw-semibold lh-sm">{{ item.name }}</div>

    <div class="mt-2 d-flex flex-wrap align-items-center gap-2">
      <span class="fw-bold">{{ item.currency }} {{ item.price }}</span>
      <span v-if="item.originalPrice" class="text-secondary text-decoration-line-through small">
        {{ item.currency }} {{ item.originalPrice }}
      </span>
      <span v-if="item.discountPercent" class="badge text-bg-success">-{{ item.discountPercent }}%</span>
      <span v-if="item.allowCoupons" class="badge text-bg-secondary">Coupons</span>
    </div>

    <div v-if="item.ratingCount" class="text-secondary small mt-2">
      Rating: {{ item.ratingAvg.toFixed(1) }} ({{ item.ratingCount }})
    </div>
  </RouterLink>
</template>

