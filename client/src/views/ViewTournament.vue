<script setup lang="ts">
import { onMounted, ref } from 'vue'
//import { useRoute } from 'vue-router'

import TournamentHeader from '../components/tournament/TournamentHeader.vue'
import TournamentTabs from '../components/tournament/TournamentTabs.vue'
import TournamentOverview from '../components/tournament/TournamentOverview.vue'
import TournamentParticipants from '../components/tournament/TournamentParticipants.vue'
import TournamentBracket from '../components/tournament/TournamentBracket.vue'
import TournamentOtherEvents from '../components/tournament/TournamentOtherEvents.vue'
import TournamentSkeleton from '../components/tournament/TournamentSkeleton.vue'
import TournamentError from '../components/tournament/TournamentError.vue'
import AppHeader from '../components/AppHeader.vue';
import SiteFooter from '../components/SiteFooter.vue';
//import type { BracketRound } from '../types/Bracket'

import type { ITournament } from '../types/Tournament'

//const route = useRoute()

const tournament = ref<ITournament | null>(null)

const loading = ref(true)
const error = ref(false)

const activeTab = ref('overview')

//const bracket = ref<BracketRound[]>([])


// TODO:
// connect auth store
const currentUser = ref(null)

//onMounted(async () => {
  //try {
    //loading.value = true

    //const id = route.params.id as string

    //console.log('ROUTE PARAMS:', route.params)
    //console.log('ID:', id)

    //const tournamentData =
      //await tournamentService.getTournamentById(id)

    //tournament.value = tournamentData

    //bracket.value = tournamentData.matches || []
  //} catch (e) {
    //console.error(e)
    //error.value = true
  //} finally {
    //loading.value = false
  //}
//})

onMounted(async () => {
  tournament.value = {
    id: '1',
    title: 'Chess Tournament',
    description: `
      The Chess Tournament is a competitive event where the best players battle for victory and prizes.
      Participants will compete in multiple rounds with a single elimination format.
      Expect intense matches, strategic gameplay, and unforgettable moments.
      Before registering for the tournament, make sure you meet all the requirements and agree to the rules.
      `,
    conditions: `
      Requirements

      • Minimum age 18 years
      • Working microphone
      • Valid game account
      • Stable internet connection

      Rules

      • No cheating or exploiting
      • Be respectful to other players
      • Follow tournament schedule
      • Use official tournament server
    `,
    startDate: '2026-05-15 16:18:',
    endDate: '2026-05-16 16:18',
    registrationCloseDate: '2026-05-10 16:18',
    sportId: '1',
    sportName: 'Chess',
    maxParticipants: 16,
    status: 'Active',
    organizerId: '1',
    organizerName: 'Admin',
    backgroundImg:
      'https://images.steamusercontent.com/ugc/2492263902782461958/8A4E6A82E6B96C9E51B0E6A6A4C36E70A6AEB1A5/',
    participantsCount: 8,
    matches: [],
  };
  loading.value = false
});
</script>

<template>
  <main class="page">
    <AppHeader />
    <TournamentSkeleton v-if="loading" />

    <TournamentError v-else-if="error" />

    <template v-else-if="tournament">
      <TournamentHeader
        :tournament="tournament"
        :current-user="currentUser"
      />

      <TournamentTabs
        :active-tab="activeTab"
        @change="activeTab = $event"
      />

      <TournamentOverview
        v-if="activeTab === 'overview'"
        :tournament="tournament"
      />

      <TournamentParticipants
        v-if="activeTab === 'participants'"
      />

      <TournamentBracket
        v-if="activeTab === 'grid'"
      />

      <TournamentOtherEvents
        v-if="activeTab === 'events'"
      />
    </template>
    
  </main>
  <SiteFooter />
</template>

<style scoped>
.page {
  min-height: 100vh;
}

@media (max-width: 768px) {
  .page {
    padding: 16px;
  }
}
</style>