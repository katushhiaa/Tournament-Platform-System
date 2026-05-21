<template>
  <RoleDashboard
    hero-subtitle="Welcome back! Explore tournaments or track your progress."
    active-subtitle="Tournaments you're currently playing in"
    my-subtitle="Your tournaments"
    :show-create-button="false"
  />
</template>

<script setup lang="ts">
import RoleDashboard from '../components/dashboard/RoleDashboard.vue'
</script>