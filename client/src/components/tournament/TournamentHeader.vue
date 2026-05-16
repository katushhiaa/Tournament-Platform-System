<script setup lang="ts">
import { useRouter } from 'vue-router'

import type { ITournament } from '../../types/Tournament'
import chessCard from '../../assets/chess-card.png'

defineProps<{
  tournament: ITournament
  currentUser?: unknown
}>()

const router = useRouter()

const handleEditTournament = (id: string) => {
  router.push(`/tournaments/${id}/edit`)
}
</script>

<template>
  <section class="header">
    <img
      :src="chessCard"
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

          <p>
            Date start:
            {{ tournament.startDate }}
          </p>

          <p>
            Date end:
            {{ tournament.endDate }}
          </p>

          <p>
            End of registration:
            {{ tournament.registrationCloseDate }}
          </p>

          <p>
            Participants:
            {{ tournament.maxParticipants }}
          </p>
        </div>

        <div class="actions">
          <div class="count">
            {{ tournament.participantsCount }}/{{ tournament.maxParticipants }}
          </div>

          <button
            class="button"
            @click="handleEditTournament(tournament.id)"
          >
            Edit Tournament
          </button>
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

  margin-left: 32px;
}

.top {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.title {
  font-size: 32px;
  font-weight: 600;
  color: white;

  margin: 0;
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
}

.details {
  max-width: 373px;
}

.details p {
  font-size: 14px;
  color: white;

  margin-bottom: 20px;
}

.actions {
  display: flex;
  flex-direction: column;
  align-items: center;

  margin-top: -20px;
}

.count {
  font-size: 12px;
  color: #ff9800;

  margin-bottom: 7px;
}

.button {
  width: 166px;
  height: 46px;

  border: none;
  border-radius: 12px;

  background: #ff9800;
  color: white;

  font-size: 13px;
  font-weight: 500;

  cursor: pointer;
}

.button:hover {
  opacity: 0.9;
}

@media (max-width: 1200px) {
  .header {
    flex-direction: column;
  }

  .right {
    margin-left: 0;
    margin-top: 32px;
    width: 100%;
  }

  .top {
    flex-direction: column;
    gap: 16px;
  }

  .bottom {
    flex-direction: column;
  }

  .actions {
    margin-top: 32px;
    align-items: flex-start;
  }
}
</style>