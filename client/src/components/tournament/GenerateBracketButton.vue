<script setup lang="ts">
import { computed, ref } from 'vue'
import { tournamentService } from '../../services/tournamentService'

const props = defineProps<{
  isOrganizer: boolean
  tournamentStatus: string
  tournamentId: string
}>()

const emit = defineEmits<{
  generated: []
}>()

const loading = ref(false)

const canGenerate = computed(() =>
  props.isOrganizer &&
  props.tournamentStatus === 'registration_closed'
)


const handleGenerate = async () => {
  const confirmed = window.confirm('Generate tournament bracket?')
  if (!confirmed) return

  try {
    loading.value = true
    await tournamentService.startTournament(props.tournamentId)
    emit('generated')
  } catch (e: any) {
    if (e?.errorCode === 'CONFLICT') {
      alert('Not enough participants or tournament already started.')
    } else {
      alert('Failed to generate bracket. Please try again.')
    }
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