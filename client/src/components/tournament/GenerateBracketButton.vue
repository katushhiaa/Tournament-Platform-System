<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps<{
  isOrganizer: boolean
  tournamentStatus: string
}>()

const emit = defineEmits<{
  generated: []
}>()

const loading = ref(false)

const canGenerate =
  props.isOrganizer &&
  props.tournamentStatus ===
    'registration_closed'

const handleGenerate = async () => {
  const confirmed = window.confirm(
    'Generate tournament bracket?',
  )

  if (!confirmed) return

  try {
    loading.value = true

    /*
      mock request
    */

    await new Promise((resolve) =>
      setTimeout(resolve, 1500),
    )

    emit('generated')
  } catch (e) {
    console.error(e)

    alert('Failed to generate bracket')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <button
    v-if="canGenerate"
    class="generate-btn"
    :disabled="loading"
    @click="handleGenerate"
  >
    {{
      loading
        ? 'Generating...'
        : 'Generate Player Grid'
    }}
  </button>
</template>

<style scoped>
.generate-btn {
  height: 52px;

  padding: 0 28px;

  border-radius: 14px;
  border: 2px solid #ff9d00;

  background: transparent;

  color: #ff9d00;

  font-size: 15px;
  font-weight: 600;

  cursor: pointer;

  transition:
    background 0.2s ease,
    color 0.2s ease,
    opacity 0.2s ease;
}

.generate-btn:hover {
  background: #ff9d00;

  color: #111827;
}

.generate-btn:disabled {
  opacity: 0.6;

  cursor: not-allowed;
}
</style>