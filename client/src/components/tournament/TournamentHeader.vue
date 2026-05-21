<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

import GenerateBracketButton from './GenerateBracketButton.vue'
import type { ITournament } from '../../types/Tournament'
import type { Participant } from '../../types/Participant'
import { useAuthStore } from '../../stores/authStore'
import { participationService } from '../../services/participationService'

import defaultBg from '../../assets/hero-card.jpg'

const props = defineProps<{
  tournament: ITournament
  participants: Participant[]
  currentUser?: { id: string; role: string } | null
}>()

const emit = defineEmits<{
  'refresh-bracket': []
  'refresh-participants': []
}>()

const router = useRouter()
const authStore = useAuthStore()

// --- Стан ---
const isJoining = ref(false)
const actionError = ref<string | null>(null)

// --- Обчислення ---
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

//видалити
const isAlreadyRegistered = computed(() =>
  props.participants.some(
    p => p.name === authStore.currentUser?.email
  )
)

//розкомітити після видалення вище
/*const isAlreadyRegistered = computed(() =>
    props.participants.some(p => p.userId === authStore.currentUser?.userId)
)*/

const showSubmitButton = computed(() =>
  !isOrganizer.value && !isAlreadyRegistered.value &&
  props.tournament.status === 'registration_open'
)

const showCancelButton = computed(() =>
  !isOrganizer.value && isAlreadyRegistered.value
)

// --- Дії ---
const handleJoin = async () => {
  
  if (isGuest.value) {
    router.push('/login')
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
  } catch (e: any) {
    if (e?.response?.status === 409) {
      actionError.value = 'You are already registered or the tournament is full.'
    } else {
      actionError.value = 'Failed to join. Please try again.'
    }
  } finally {
    isJoining.value = false
  }
}

const handleEditTournament = () => {
  router.push(`/tournaments/${props.tournament.id}/edit`)
}
</script>

<template>
  <section class="header">
    <img
      :src="coverImage"
      alt="Tournament"
      class="image"
    />

    <div class="right">
      <div class="top">
        <h1 class="title">
          {{ tournament.title }}
        </h1>

        <div class="format">
          Format: Single Elimination
        </div>
      </div>

      <div class="bottom">
        <div class="details">
          <p>
            Sport Type:
            {{ tournament.sportName }}
          </p>

          <p>Date start: {{ formatDate(tournament.startDate) }}</p>
          <p>Date end: {{ formatDate(tournament.endDate) }}</p>
          <p>End of registration: {{ formatDate(tournament.registrationCloseDate) }}</p>

          <p>
            Participants:
            {{ tournament.maxParticipants }}
          </p>
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

            <!-- Організатор: Edit (тільки якщо його турнір) -->
            <button
              v-if="isOrganizer"
              class="button"
              @click="handleEditTournament"
            >
              Edit Tournament
            </button>

            <!-- Гравець / Гість: Submit -->
            <button
              v-if="showSubmitButton"
              class="button button--join"
              :disabled="isJoining"
              @click="handleJoin"
            >
              {{ isJoining ? 'Joining...' : 'Submit an application' }}
            </button>

            <!-- Гравець: Cancel (тільки до старту) -->
            <button
              v-if="showCancelButton"
              class="button button--cancel"
              disabled
            >
              Cancel participation
            </button>

            <p v-if="actionError" class="action-error">{{ actionError }}</p>
          </div>
        </div>
      </div>
    </div>
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

.right {
  flex: 1;
}

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

.bottom {
  margin-top: 30px;

  display: flex;
  justify-content: space-between;
  align-items: flex-start;

  gap: 32px;
}

.details {
  max-width: 373px;
}

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

.button:hover {
  opacity: 0.9;
}

.button--join {
  background: #ff9800;
  border: none;
}

.button--cancel {
  background: transparent;
  border: 2px solid #e57373;
  color: #e57373;
}

.button--cancel:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.button--join:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.action-error {
  color: #e57373;
  font-size: 13px;
  margin-top: 8px;
  width: 100%;
}

@media (max-width: 1200px) {
  .header {
    margin-left: 24px;
    margin-right: 24px;

    flex-direction: column;
  }

  .right {
    width: 100%;
  }

  .top {
    flex-direction: column;

    gap: 16px;
  }

  .bottom {
    flex-direction: column;
  }

  .side {
    align-items: flex-start;
  }

  .buttons {
    flex-wrap: wrap;
  }
}

@media (max-width: 768px) {
  .header {
    padding-top: 140px;
  }

  .image {
    width: 100%;
    height: auto;
  }

  .buttons {
    width: 100%;

    flex-direction: column;
    align-items: stretch;
  }

  .button {
    width: 100%;
  }
}
</style>