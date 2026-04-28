<template>
  <RoleDashboard
    hero-subtitle="Welcome back! Ready to compete and create unforgettable moments?"
    active-subtitle="Tournaments you are currently participating in"
    my-subtitle="Tournaments you have created or organized"
    :show-create-button="true"
  />
</template>

<script setup lang="ts">
import RoleDashboard from '../components/dashboard/RoleDashboard.vue';
</script>