<template>
  <main class="login-page" :style="{ backgroundImage: `url(${heroBg})` }">
    <div class="login-page__overlay"></div>

    <div class="login-page__header">
      <AppHeader />
    </div>

    <LoginForm />
  </main>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue';
import LoginForm from '../components/forms/LoginForm.vue';
import heroBg from '../assets/hero-bg.png';
</script>

<style scoped>
.login-page {
  position: relative;
  min-height: 100vh;
  overflow-x: hidden;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.login-page__overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 8, 24, 0.45);
}

.login-page__header {
  position: relative;
  z-index: 2;
}

.login-page :deep(.header) {
  position: absolute;
  top: 22px;
  left: 28px;
  right: 28px;
  width: auto;
  max-width: none;
}

.login-page :deep(.login-form) {
  position: relative;
  z-index: 1;
  padding-top: 170px;
}

@media (max-width: 768px) {
  .login-page :deep(.header) {
    position: static;
    margin: 16px;
  }

  .login-page :deep(.login-form) {
    padding-top: 56px;
  }
}
</style>