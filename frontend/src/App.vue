<script setup>
import { useThemeStore } from './stores/theme'

// Ensure theme store is instantiated at app start
useThemeStore()
</script>

<template>
  <router-view />
</template>
