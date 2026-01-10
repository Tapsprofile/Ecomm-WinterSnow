<script setup>
import { useThemeStore } from '../../stores/theme'

const theme = useThemeStore()
</script>

<template>
  <button class="btn" type="button" @click="theme.toggle()">
    Theme: {{ theme.mode }} ({{ theme.roleKey }})
  </button>
</template>

