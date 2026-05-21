<template>
  <div class="not-found">
    <AppHeader />
    <div class="not-found__content">
      <h1 class="not-found__code">404</h1>
      <p class="not-found__message">Page not found</p>
      <RouterLink to="/" class="not-found__button">Go to Home</RouterLink>
    </div>
  </div>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
</script>

<style scoped>
.not-found {
  min-height: 100vh;
  background: #151d22;
  color: #fffcf2;
}

.not-found__content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 80vh;
  gap: 16px;
}

.not-found__code {
  font-size: 120px;
  font-weight: 900;
  color: #1531ce;
  margin: 0;
}

.not-found__message {
  font-size: 24px;
  opacity: 0.8;
  margin: 0;
}

.not-found__button {
  margin-top: 16px;
  padding: 12px 32px;
  border: 1px solid #ff9800;
  border-radius: 12px;
  background: #ff9800;
  color: #fffcf2;
  font-size: 16px;
  font-weight: 700;
  text-decoration: none;
}
</style>