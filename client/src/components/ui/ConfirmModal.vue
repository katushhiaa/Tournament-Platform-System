<script setup lang="ts">
defineProps<{
  title?: string
  message: string
  confirmText?: string
  cancelText?: string
  confirmDanger?: boolean
  loading?: boolean
}>()

defineEmits<{
  confirm: []
  cancel: []
}>()
</script>

<template>
  <div class="overlay" @mousedown.self="$emit('cancel')">
    <div class="modal">
      <div class="modal__icon">
        <svg width="56" height="56" viewBox="0 0 56 56" fill="none">
          <path
            d="M28 6 L51 46 H5 Z"
            stroke="#ff9800"
            stroke-width="2"
            stroke-linejoin="round"
            fill="none"
          />
          <path d="M28 22v12M28 38v2" stroke="#ff9800" stroke-width="2.5" stroke-linecap="round" />
        </svg>
      </div>

      <h2 class="modal__title">{{ title ?? 'Confirmation' }}</h2>

      <p class="modal__message">{{ message }}</p>

      <div class="modal__actions">
        <button
          class="modal__btn modal__btn--no"
          :disabled="loading"
          @click="$emit('cancel')"
        >
          {{ cancelText ?? 'No' }}
        </button>

        <button
          class="modal__btn"
          :class="confirmDanger ? 'modal__btn--danger' : 'modal__btn--yes'"
          :disabled="loading"
          @click="$emit('confirm')"
        >
          {{ loading ? '...' : (confirmText ?? 'Yes') }}
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  inset: 0;
  z-index: 1000;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}

.modal {
  background: #0f1923;
  border: 1px solid #1531ce;
  border-radius: 20px;
  padding: 48px 56px;
  max-width: 480px;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  text-align: center;
}

.modal__icon {
  margin-bottom: 4px;
}

.modal__title {
  font-size: 28px;
  font-weight: 700;
  color: #fff;
  margin: 0;
}

.modal__message {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.6;
  margin: 0;
}

.modal__actions {
  display: flex;
  gap: 16px;
  margin-top: 8px;
}

.modal__btn {
  width: 120px;
  height: 44px;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.2s;
}

.modal__btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.modal__btn--no {
  border: 1px solid rgba(255, 255, 255, 0.3);
  background: transparent;
  color: rgba(255, 255, 255, 0.7);
}

.modal__btn--yes {
  border: none;
  background: #ff9800;
  color: #fff;
}

.modal__btn--danger {
  border: none;
  background: #ce0f0f;
  color: #fff;
}

.modal__btn--yes:hover:not(:disabled),
.modal__btn--danger:hover:not(:disabled) {
  opacity: 0.88;
}
</style>