<script setup lang="ts">
import { computed, ref } from 'vue'
import { tournamentService } from '../../services/tournamentService'
import ConfirmModal from '../ui/ConfirmModal.vue'
import AppToast from '../ui/AppToast.vue'

const props = defineProps<{
  isOrganizer: boolean
  tournamentStatus: string
  tournamentId: string
}>()

const emit = defineEmits<{ generated: [] }>()

const loading = ref(false)
const showConfirm = ref(false)
const toast = ref('')
const toastType = ref<'success' | 'error'>('error')

const canGenerate = computed(() =>
  props.isOrganizer && props.tournamentStatus === 'registration_closed'
)

const handleGenerate = async () => {
  showConfirm.value = false
  try {
    loading.value = true
    await tournamentService.startTournament(props.tournamentId)
    emit('generated')
  } catch (e: any) {
    toastType.value = 'error'
    if (e?.errorCode === 'CONFLICT') {
      toast.value = 'Not enough participants or tournament already started.'
    } else {
      toast.value = 'Failed to generate bracket. Please try again.'
    }
    setTimeout(() => { toast.value = '' }, 4000)
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
    @click="showConfirm = true"
  >
    {{ loading ? 'Generating...' : 'Generate Player Grid' }}
  </button>

  <ConfirmModal
    v-if="showConfirm"
    message="Generate tournament bracket? This action cannot be undone."
    confirm-text="Generate"
    @confirm="handleGenerate"
    @cancel="showConfirm = false"
  />

  <AppToast v-if="toast" :message="toast" :type="toastType" />
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
  transition: background 0.2s ease, color 0.2s ease, opacity 0.2s ease;
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