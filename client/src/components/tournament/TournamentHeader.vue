<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import GenerateBracketButton from './GenerateBracketButton.vue'
import type { ITournament } from '../../types/Tournament'

import defaultBg from '../../assets/chess-card.png'

const props = defineProps<{
  tournament: ITournament
  currentUser?: { id: string; role: string } | null
}>()

const emit = defineEmits<{
  'refresh-bracket': []
}>()

const router = useRouter()

const isOrganizer = computed(() =>
  props.currentUser?.id === props.tournament.organizerId
)

const coverImage = computed(() =>
  props.tournament.backgroundImg ?? defaultBg
)

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })

const handleEditTournament = (id: string) => {
  router.push(`/tournaments/${id}/edit`)
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
              :is-organizer="isOrganizer"
              :tournament-status="
                tournament.status
              "
              @generated="
                emit('refresh-bracket')
              "
            />

            <button
              class="button"
              @click="
                handleEditTournament(
                  tournament.id,
                )
              "
            >
              Edit Tournament
            </button>
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