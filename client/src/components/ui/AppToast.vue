<script setup lang="ts">
defineProps<{
  message: string
  type?: 'success' | 'error'
}>()
</script>

<template>
  <div class="toast" :class="`toast--${type ?? 'success'}`">
    {{ message }}
  </div>
</template>

<style scoped>
.toast {
  position: fixed;
  bottom: 32px;
  left: 50%;
  transform: translateX(-50%);
  padding: 14px 32px;
  border-radius: 40px;
  font-size: 16px;
  font-weight: 700;
  z-index: 9999;
  white-space: nowrap;
  border: 1px solid;
}

.toast--success {
  background: rgba(22, 60, 22, 0.92);
  border-color: #84c082;
  color: #84c082;
}

.toast--error {
  background: rgba(80, 16, 16, 0.92);
  border-color: #ce0f0f;
  color: #ce0f0f;
}
</style>