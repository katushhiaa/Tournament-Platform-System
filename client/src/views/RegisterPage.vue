<template>
  <section class="register-page" :style="{ backgroundImage: `url(${registerBg})` }">
    <div class="register-page__overlay"></div>

    <AppHeader />

    <div class="register-page__content">
      <RegisterForm />
    </div>
  </section>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue';
import RegisterForm from '../components/forms/RegisterForm.vue';
import registerBg from '../assets/hero-bg.png';
</script>

<style scoped>
.register-page {
  position: relative;
  min-height: 100vh;
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

.register-page__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(37, 46, 53, 0.22) 0%,
    rgba(37, 46, 53, 0.34) 40%,
    rgba(37, 46, 53, 0.58) 100%
  );
}

.register-page__content {
  position: relative;
  z-index: 1;
}
</style>