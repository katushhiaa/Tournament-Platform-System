<script setup lang="ts">
import type {
  IBracketStructure,
} from '../../types/Bracket'

defineProps<{
  rounds: IBracketStructure
}>()
</script>

<template>
  <section class="bracket">
    <h2 class="title">
      Tournament bracket
    </h2>

    <div
      v-if="!rounds.length"
      class="empty"
    >
      Bracket has not been generated yet
    </div>

    <div
      v-else
      class="rounds"
    >
      <div
        v-for="round in rounds"
        :key="round.round"
        class="round"
      >
        <h3 class="round-title">
          {{ round.roundDisplayName }}
        </h3>

        <div
          v-for="match in round.matches"
          :key="match.matchId"
          class="match"
        >
          <div class="player">
            {{ match.player1Id || 'TBD' }}
          </div>

          <div class="player">
            {{ match.player2Id || 'TBD' }}
          </div>

          <div class="status">
            {{ match.status }}
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.bracket {
  padding: 90px 80px 140px;
}

.title {
  text-align: center;
  font-size: 32px;
  font-weight: 600;
  color: white;
  margin-bottom: 60px;
}

.empty {
  text-align: center;
  color: rgba(255,255,255,0.7);
  font-size: 18px;
}

.rounds {
  display: flex;
  gap: 40px;
  overflow-x: auto;
}

.round {
  min-width: 260px;
}

.round-title {
  text-align: center;
  margin-bottom: 24px;
  color: white;
}

.match {
  background: rgba(21, 49, 206, 0.35);
  border: 1px solid #1531ce;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 24px;
}

.player {
  background: #1531ce;
  border-radius: 8px;
  padding: 12px;
  color: white;
}

.player + .player {
  margin-top: 12px;
}

.status {
  margin-top: 12px;
  text-align: center;
  color: rgba(255,255,255,0.7);
  font-size: 14px;
}
</style>