<script setup lang="ts">
import { onErrorCaptured, ref } from 'vue'

const hasError = ref(false)
const errorMessage = ref('')

onErrorCaptured((err: unknown) => {
  hasError.value = true
  errorMessage.value = err instanceof Error ? err.message : 'Something went wrong.'
  return false
})

const reset = () => {
  hasError.value = false
  errorMessage.value = ''
}
</script>

<template>
  <div v-if="hasError" class="error-boundary">
    <p class="error-boundary__message">{{ errorMessage }}</p>
    <button class="error-boundary__btn" @click="reset">Try again</button>
  </div>
  <slot v-else />
</template>

<style scoped>
.error-boundary {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 48px 24px;
  gap: 16px;
}

.error-boundary__message {
  color: #fffcf2;
  font-size: 16px;
  opacity: 0.8;
  text-align: center;
}

.error-boundary__btn {
  padding: 10px 28px;
  border-radius: 18px;
  border: 1px solid #ff9800;
  background: transparent;
  color: #ff9800;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
}

.error-boundary__btn:hover {
  background: rgba(255, 152, 0, 0.1);
}
</style>