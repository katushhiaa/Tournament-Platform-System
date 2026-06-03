<template>
  <div class="skeleton-page">
    <div class="skeleton-hero">
      <div class="skeleton-block skeleton-block--title" />
      <div class="skeleton-block skeleton-block--subtitle" />
      <div class="skeleton-actions">
        <div class="skeleton-block skeleton-block--btn" />
        <div class="skeleton-block skeleton-block--btn" />
      </div>
    </div>

    <div class="skeleton-tabs">
      <div v-for="n in 4" :key="n" class="skeleton-block skeleton-block--tab" />
    </div>

    <div class="skeleton-body">
      <div class="skeleton-block skeleton-block--text" />
      <div class="skeleton-block skeleton-block--text skeleton-block--text-short" />
      <div class="skeleton-block skeleton-block--text" />
    </div>
  </div>
</template>

<style scoped>
.skeleton-page {
  min-height: 100vh;
  background: transparent;
}

.skeleton-hero {
  min-height: 340px;
  padding: 80px 40px 48px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
}

.skeleton-tabs {
  display: flex;
  gap: 12px;
  padding: 24px 40px;
  border-bottom: 1px solid rgba(255,255,255,0.1);
}

.skeleton-body {
  padding: 48px 80px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  max-width: 860px;
  margin: 0 auto;
}

.skeleton-block {
  border-radius: 10px;
  background: linear-gradient(90deg, rgba(255,255,255,0.06) 25%, rgba(255,255,255,0.12) 50%, rgba(255,255,255,0.06) 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}

.skeleton-block--title {
  width: 360px;
  height: 48px;
}

.skeleton-block--subtitle {
  width: 220px;
  height: 24px;
  opacity: 0.7;
}

.skeleton-block--btn {
  width: 160px;
  height: 44px;
  border-radius: 12px;
}

.skeleton-actions {
  display: flex;
  gap: 16px;
  margin-top: 8px;
}

.skeleton-block--tab {
  width: 120px;
  height: 36px;
  border-radius: 8px;
}

.skeleton-block--text {
  width: 100%;
  height: 20px;
  border-radius: 6px;
}

.skeleton-block--text-short {
  width: 60%;
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

@media (max-width: 768px) {
  .skeleton-hero {
    padding: 60px 20px 32px;
  }

  .skeleton-body {
    padding: 32px 20px;
  }

  .skeleton-block--title {
    width: 240px;
  }
}
</style>