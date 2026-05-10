<script setup lang="ts">
import { onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useBracket } from '../composables/useBracket';

const route = useRoute();
const tournamentId = route.params.id as string;

const {
  bracket,
  loading,
  error,
  loadBracket,
} = useBracket();

onMounted(() => {
  loadBracket(tournamentId);
});
</script>

<template>
  <div>
    <h1>Bracket</h1>

    <p v-if="loading">Loading...</p>
    <p v-if="error">{{ error }}</p>

    <div v-if="bracket">
      <div
        v-for="round in bracket.rounds"
        :key="round.round"
      >
        <h2>Round {{ round.round }}</h2>

        <div
          v-for="match in round.matches"
          :key="match.matchId"
        >
          <p>
            {{ match.player1Id }} vs
            {{ match.player2Id }}
          </p>

          <small>{{ match.status }}</small>
        </div>
      </div>
    </div>
  </div>
</template>