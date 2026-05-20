<script setup lang="ts">
import type { Participant }
from '../../types/Participant'

defineProps<{
  participants: Participant[]
}>()
</script>

<template>
  <section class="participants">
    <h2 class="title">
      Participants
    </h2>

    <div class="list">
      <div
        v-for="participant in participants"
        :key="participant.id"
        class="participant-card"
      >
        {{ participant.name }}
      </div>
    </div>

    <div class="pagination">
      <button class="page active">
        1
      </button>

      <button class="page">
        2
      </button>

      <button class="page">
        3
      </button>

      <button class="page">
        4
      </button>

      <button class="page">
        5
      </button>

    </div>
  </section>
</template>

<style scoped>
.participants {
  margin-top: 104px;

  display: flex;
  flex-direction: column;
  align-items: center;
}

.title {
  font-size: 32px;
  font-weight: 600;
  color: white;

  margin-bottom: 57px;
}

.list {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.participant-card {
  width: 756px;
  height: 55px;

  border-radius: 12px;
  border: 1px solid #1531ce;

  background: rgba(21, 49, 206, 0.35);

  display: flex;
  align-items: center;

  padding-left: 34px;

  color: white;
  font-size: 16px;

  backdrop-filter: blur(6px);
}

.pagination {
  margin-top: 55px;
  margin-bottom: 74px;
  display: flex;
  gap: 12px;
}

.page {
  width: 28px;
  height: 28px;

  border-radius: 6px;
  border: 1px solid #1531ce;

  background: transparent;

  color: #1531ce;
  font-size: 12px;

  cursor: pointer;
}

.page.active {
  background: #1531ce;
  color: white;
}

@media (max-width: 900px) {
  .participant-card {
    width: 90vw;
  }
}
</style>