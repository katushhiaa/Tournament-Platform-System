<script setup lang="ts">
defineEmits<{ retry: [] }>()
</script>

<template>
  <div class="error">
    <p class="error__text">Failed to load tournament.</p>
    <button class="error__retry" @click="$emit('retry')">Retry</button>
  </div>
</template>

<style scoped>
.error {
  background: #1e293b;
  padding: 24px;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.error__text {
  color: rgba(255, 255, 255, 0.7);
  font-size: 16px;
}

.error__retry {
  height: 40px;
  padding: 0 24px;
  border-radius: 10px;
  border: 1px solid #1531ce;
  background: transparent;
  color: #1531ce;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}

.error__retry:hover {
  background: #1531ce;
  color: white;
}
</style>