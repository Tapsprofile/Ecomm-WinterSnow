<script setup>
defineProps({
  item: { type: Object, required: true }
})
</script>

<template>
  <RouterLink class="card" :to="`/p/${item.slug}`" style="display:block;">
    <div style="aspect-ratio: 4 / 3; overflow:hidden; border-radius:10px; background:#f1f5f9; border:1px solid #e2e8f0;">
      <img
        v-if="item.thumbnailUrl"
        :src="item.thumbnailUrl"
        alt=""
        style="width:100%; height:100%; object-fit:cover;"
        loading="lazy"
      />
      <div v-else class="muted" style="padding:12px;">No image</div>
    </div>
    <div style="margin-top:10px; font-weight:700; line-height:1.2;">{{ item.name }}</div>
    <div style="margin-top:6px;">
      <span style="font-weight:900;">{{ item.currency }} {{ item.price }}</span>
      <span v-if="item.originalPrice" class="muted" style="margin-left:8px; text-decoration:line-through;">
        {{ item.currency }} {{ item.originalPrice }}
      </span>
      <span v-if="item.discountPercent" class="pill" style="margin-left:8px;">-{{ item.discountPercent }}%</span>
      <span v-if="item.allowCoupons" class="pill" style="margin-left:6px;">Coupons</span>
    </div>
    <div v-if="item.ratingCount" class="muted" style="margin-top:6px; font-size:12px;">
      Rating: {{ item.ratingAvg.toFixed(1) }} ({{ item.ratingCount }})
    </div>
  </RouterLink>
</template>

