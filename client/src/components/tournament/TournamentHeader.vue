<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

import GenerateBracketButton from './GenerateBracketButton.vue'
import type { ITournament } from '../../types/Tournament'
import type { Participant } from '../../types/Participant'
import { useAuthStore } from '../../stores/authStore'
import { participationService } from '../../services/participationService'
import ConfirmModal from '../ui/ConfirmModal.vue'
import defaultBg from '../../assets/hero-card.png'

const props = defineProps<{
  tournament: ITournament
  participants: Participant[]
  currentUser?: { id: string; role: string } | null
}>()

const emit = defineEmits<{
  'refresh-bracket': []
  'refresh-participants': []
  'show-toast': [message: string]
}>()

const router = useRouter()
const authStore = useAuthStore()

const isJoining = ref(false)
const isCancelling = ref(false)
const actionError = ref<string | null>(null)
const showCancelConfirm = ref(false)

const coverImage = computed(() =>
  props.tournament.backgroundImg ?? defaultBg
)

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })

const isOrganizer = computed(() =>
  props.currentUser?.id === props.tournament.organizerId
)

const isGuest = computed(() => !authStore.isAuthenticated)

const isAlreadyRegistered = computed(() =>
  props.participants.some(p => p.userId === authStore.currentUser?.userId)
)

const canCancel = computed(() => {
  const status = props.tournament.status
  const regClose = new Date(props.tournament.registrationCloseDate)
  return (
    isAlreadyRegistered.value &&
    !isOrganizer.value &&
    status !== 'in_progress' &&
    status !== 'completed' &&
    status !== 'registration_closed' &&
    new Date() < regClose
  )
})

const showSubmitButton = computed(() =>
  !isOrganizer.value &&
  !isAlreadyRegistered.value &&
  props.tournament.status === 'registration_open'
)

const handleJoin = async () => {
  if (isGuest.value) {
    actionError.value = 'You need to log in to participate in a tournament.'
    return
  }

  try {
    isJoining.value = true
    actionError.value = null
    await participationService.addParticipant(
      props.tournament.id,
      authStore.currentUser!.userId,
    )
    emit('refresh-participants')
    emit('show-toast', `You have successfully joined ${props.tournament.title}!`)
  } catch (e: any) {
    const status = e?.response?.status
    const msg = e?.response?.data?.error?.message

    if (status === 400) {
      actionError.value = 'Tournament registration is already closed.'
    } else if (status === 401) {
      actionError.value = 'You need to log in to participate in a tournament.'
    } else if (status === 404) {
      actionError.value = 'Tournament not found.'
    } else if (status === 409) {
      actionError.value = msg ?? 'You are already registered or the tournament is full.'
    } else if (status === 500) {
      actionError.value = 'Internal server error. Please try again later.'
    } else if (!status) {
      actionError.value = 'Connection error. Please try again.'
    } else {
      actionError.value = 'Failed to join. Please try again.'
    }
  } finally {
    isJoining.value = false
  }
}

const handleCancelConfirm = async () => {
  try {
    isCancelling.value = true
    actionError.value = null
    await participationService.leaveParticipant(props.tournament.id)
    showCancelConfirm.value = false
    emit('refresh-participants')
    emit('show-toast', 'You have successfully cancelled your participation.')
  } catch (e: any) {
    const status = e?.response?.status
    const msg = e?.response?.data?.error?.message
    showCancelConfirm.value = false

    if (status === 400) {
      actionError.value = msg ?? 'Registration is already closed.'
    } else if (status === 409) {
      actionError.value = msg ?? 'You are not a participant of this tournament.'
    } else {
      actionError.value = 'Failed to cancel participation. Please try again.'
    }
  } finally {
    isCancelling.value = false
  }
}

const handleEditTournament = () => {
  router.push(`/tournaments/${props.tournament.id}/edit`)
}
</script>

<template>
  <section class="header">
    <img :src="coverImage" alt="Tournament" class="image" />

    <div class="right">
      <div class="top">
        <h1 class="title">
          {{ tournament.title }}
        </h1>

        <div class="right-top">
          <div class="status-badge" :class="`status-badge--${tournament.status}`">
            {{ tournament.status.replace(/_/g, ' ').toUpperCase() }}
          </div>
          <div class="format">
            Format: Single Elimination
          </div>
        </div>
      </div>

      <div class="bottom">
        <div class="details">
          <p>Sport Type: {{ tournament.sportName }}</p>
          <p>Date start: {{ formatDate(tournament.startDate) }}</p>
          <p>Date end: {{ formatDate(tournament.endDate) }}</p>
          <p>End of registration: {{ formatDate(tournament.registrationCloseDate) }}</p>
          <p>Participants: {{ tournament.maxParticipants }}</p>
        </div>

        <div class="side">
          <div class="count">
            {{ tournament.participantsCount }}/{{ tournament.maxParticipants }}
          </div>

          <div class="buttons">
            <GenerateBracketButton
              v-if="isOrganizer"
              :is-organizer="isOrganizer"
              :tournament-status="tournament.status"
              :tournament-id="tournament.id"
              @generated="emit('refresh-bracket')"
            />

            <button
              v-if="isOrganizer"
              class="button"
              @click="handleEditTournament"
            >
              Edit Tournament
            </button>

            <button
              v-if="showSubmitButton"
              class="button button--join"
              :disabled="isJoining"
              @click="handleJoin"
            >
              {{ isJoining ? 'Joining...' : 'Submit an application' }}
            </button>

            <button
              v-if="canCancel"
              class="button button--cancel"
              :disabled="isCancelling"
              @click="showCancelConfirm = true"
            >
              {{ isCancelling ? 'Cancelling...' : 'Cancel participation' }}
            </button>
          </div>

          <p v-if="actionError" class="action-error">{{ actionError }}</p>

          <p v-if="actionError && isGuest" class="action-login">
            <a @click="router.push('/login')">Log in</a> to participate
          </p>
        </div>
      </div>
    </div>

    <ConfirmModal
      v-if="showCancelConfirm"
      message="Are you sure you want to cancel your participation?"
      confirm-text="Yes, cancel"
      cancel-text="No"
      confirm-danger
      :loading="isCancelling"
      @confirm="handleCancelConfirm"
      @cancel="showCancelConfirm = false"
    />
  </section>
</template>

<style scoped>
.header {
  padding-top: 197px;
  margin-left: 80px;
  margin-right: 83px;
  display: flex;
  align-items: flex-start;
  gap: 32px;
}

.image {
  width: 373px;
  height: 244px;
  object-fit: cover;
  border-radius: 24px;
  flex-shrink: 0;
}

.right { flex: 1; }

.top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.title {
  margin: 0;
  font-size: 32px;
  font-weight: 600;
  color: white;
}

.format {
  font-size: 16px;
  color: #84c082;
  white-space: nowrap;
}

.right-top {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 8px;
}

.status-badge {
  padding: 4px 14px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.06em;
  white-space: nowrap;
}

.status-badge--registration_open {
  background: rgba(21, 49, 206, 0.35);
  border: 1px solid #1531ce;
  color: #7b9fff;
}

.status-badge--registration_closed {
  background: rgba(255, 152, 0, 0.15);
  border: 1px solid #ff9800;
  color: #ff9800;
}

.status-badge--in_progress {
  background: rgba(255, 193, 7, 0.15);
  border: 1px solid #ffc107;
  color: #ffc107;
}

.status-badge--completed {
  background: rgba(22, 101, 52, 0.35);
  border: 1px solid rgba(52, 211, 100, 0.5);
  color: #84c082;
}

.status-badge--draft {
  background: rgba(255,255,255,0.06);
  border: 1px solid rgba(255,255,255,0.2);
  color: rgba(255,255,255,0.5);
}

.bottom {
  margin-top: 30px;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 32px;
}

.details { max-width: 373px; }

.details p {
  margin-bottom: 20px;
  font-size: 14px;
  color: white;
}

.side {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.count {
  margin-bottom: 10px;
  font-size: 12px;
  color: #ffffff;
}

.buttons {
  display: flex;
  align-items: center;
  gap: 14px;
}

.button {
  width: 166px;
  height: 52px;
  border: none;
  border-radius: 14px;
  background: #ff9800;
  color: white;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s ease;
}

.button:hover { opacity: 0.9; }

.button--join {
  background: #ff9800;
  border: none;
}

.button--cancel {
  background: transparent;
  border: 2px solid #ce0f0f;
  color: #ce0f0f;
}

.button--cancel:disabled,
.button--join:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.action-error {
  color: #ce0f0f;
  font-size: 13px;
  margin-top: 10px;
  max-width: 340px;
  text-align: right;
}

.action-login {
  font-size: 13px;
  margin-top: 6px;
  color: rgba(255,255,255,0.6);
  text-align: right;
}

.action-login a {
  color: #4d6eff;
  cursor: pointer;
  text-decoration: underline;
}

@media (max-width: 1200px) {
  .header {
    margin-left: 24px;
    margin-right: 24px;
    flex-direction: column;
  }
  .right { width: 100%; }
  .top { flex-direction: column; gap: 16px; }
  .bottom { flex-direction: column; }
  .side { align-items: flex-start; }
  .buttons { flex-wrap: wrap; }
  .action-error,
  .action-login { text-align: left; }
}

@media (max-width: 768px) {
  .header { padding-top: 140px; }
  .image { width: 100%; height: auto; }
  .buttons {
    width: 100%;
    flex-direction: column;
    align-items: stretch;
  }
  .button { width: 100%; }
}
</style>