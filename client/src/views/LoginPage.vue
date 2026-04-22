<template>
  <section class="login-page">
    <div class="login-page__box">
      <h1>Login Page</h1>
      <p>This page will be implemented in the next task.</p>
    </div>
  </section>
</template>

<script setup lang="ts"></script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #252e35;
  padding: 24px;
}

.login-page__box {
  width: 100%;
  max-width: 420px;
  padding: 40px 24px;
  border: 1px solid #1531ce;
  border-radius: 20px;
  background: rgba(37, 46, 53, 0.92);
  color: #fffcf2;
  text-align: center;
}

.login-page__box h1 {
  margin: 0 0 12px;
  font-size: 32px;
}

.login-page__box p {
  margin: 0;
  font-size: 16px;
  opacity: 0.9;
}
</style>