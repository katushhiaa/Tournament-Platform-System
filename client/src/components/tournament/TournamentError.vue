<script setup lang="ts">
defineEmits<{ retry: [] }>()
</script>

<template>
  <div class="error-page">
    <div class="error-card">
      <div class="error-icon">
        <svg width="56" height="56" viewBox="0 0 56 56" fill="none">
          <circle cx="28" cy="28" r="26" stroke="#ce0f0f" stroke-width="2" />
          <path d="M28 16v16M28 36v2" stroke="#ce0f0f" stroke-width="2.5" stroke-linecap="round" />
        </svg>
      </div>

      <h2 class="error-card__title">Failed to load tournament</h2>
      <p class="error-card__text">
        Something went wrong while loading the tournament data.<br />
        Please try again or come back later.
      </p>

      <button class="error-card__retry" @click="$emit('retry')">
        Retry
      </button>
    </div>
  </div>
</template>

<style scoped>
.error-page {
  min-height: calc(100vh - 80px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 60px 24px;
}

.error-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  max-width: 480px;
  width: 100%;
  background: rgba(30, 41, 59, 0.85);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 20px;
  padding: 56px 48px;
  text-align: center;
  backdrop-filter: blur(8px);
}

.error-icon {
  margin-bottom: 4px;
}

.error-card__title {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  color: #fffcf2;
}

.error-card__text {
  margin: 0;
  font-size: 15px;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.6;
}

.error-card__retry {
  margin-top: 8px;
  height: 44px;
  padding: 0 36px;
  border-radius: 10px;
  border: 1px solid #1531ce;
  background: #1531ce;
  color: #fffcf2;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.error-card__retry:hover {
  opacity: 0.85;
}
</style>