<script setup>
const slides = [
  {
    title: 'WinterSnow Deals',
    subtitle: 'Top picks for the season — high contrast, fast checkout.',
    cta: { label: 'Shop Top Picks', to: '/search' },
    img: 'https://picsum.photos/seed/wintersnow-banner-1/1600/700'
  },
  {
    title: 'New Arrivals',
    subtitle: 'Fresh listings from verified vendors.',
    cta: { label: 'Browse New', to: '/search?q=' },
    img: 'https://picsum.photos/seed/wintersnow-banner-2/1600/700'
  },
  {
    title: 'Winter Collections',
    subtitle: 'Curated collections with discounts & coupons.',
    cta: { label: 'Explore', to: '/search' },
    img: 'https://picsum.photos/seed/wintersnow-banner-3/1600/700'
  }
]
</script>

<template>
  <div id="wsHero" class="carousel slide" data-bs-ride="carousel">
    <div class="carousel-inner rounded-3 border overflow-hidden">
      <div v-for="(s, idx) in slides" :key="s.title" class="carousel-item" :class="{ active: idx === 0 }">
        <div class="ratio ratio-21x9 bg-body-tertiary">
          <img :src="s.img" alt="" class="w-100 h-100 ws-cover" />
        </div>
        <div class="carousel-caption text-start">
          <div class="bg-body border rounded-3 p-3" style="max-width: 520px;">
            <div class="fw-bold">{{ s.title }}</div>
            <div class="text-secondary small mt-1">{{ s.subtitle }}</div>
            <RouterLink class="btn btn-primary btn-sm mt-3" :to="s.cta.to">{{ s.cta.label }}</RouterLink>
          </div>
        </div>
      </div>
    </div>

    <button class="carousel-control-prev" type="button" data-bs-target="#wsHero" data-bs-slide="prev">
      <span class="carousel-control-prev-icon" aria-hidden="true"></span>
      <span class="visually-hidden">Previous</span>
    </button>
    <button class="carousel-control-next" type="button" data-bs-target="#wsHero" data-bs-slide="next">
      <span class="carousel-control-next-icon" aria-hidden="true"></span>
      <span class="visually-hidden">Next</span>
    </button>
  </div>
</template>

