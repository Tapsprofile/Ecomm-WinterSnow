<script setup>
import { useThemeStore } from '../../stores/theme'

const theme = useThemeStore()
</script>

<template>
  <button class="btn btn-outline-secondary btn-sm" type="button" @click="theme.toggle()">
    <i class="bi bi-circle-half me-1" />
    {{ theme.mode }}
  </button>
</template>

