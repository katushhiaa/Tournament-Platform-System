<script setup lang="ts">
import { onMounted, ref } from 'vue'
// import { useRoute } from 'vue-router'

import TournamentHeader from '../components/tournament/TournamentHeader.vue'
import TournamentTabs from '../components/tournament/TournamentTabs.vue'
import TournamentOverview from '../components/tournament/TournamentOverview.vue'
import TournamentParticipants from '../components/tournament/TournamentParticipants.vue'
import TournamentBracket from '../components/tournament/TournamentBracket.vue'
import TournamentOtherEvents from '../components/tournament/TournamentOtherEvents.vue'
import TournamentSkeleton from '../components/tournament/TournamentSkeleton.vue'
import TournamentError from '../components/tournament/TournamentError.vue'

import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'

import type {
  IBracketStructure,
} from '../types/Bracket'

import type { ITournament } from '../types/Tournament'

// const route = useRoute()

const tournament = ref<ITournament | null>(null)

const loading = ref(true)
const error = ref(false)

const activeTab = ref('overview')

const bracket = ref<IBracketStructure>([])

const currentUser = ref({
  id: '1',
  role: 'organizer',
})
/*
  TODO:
  enable after backend integration
*/
//onMounted(async () => { 
  //try {
    //loading.value = true

    //const id = route.params.id as string

    //console.log('ROUTE PARAMS:', route.params)
    //console.log('ID:', id)

    //const tournamentData =
      //await tournamentService.getTournamentById(id)

    //tournament.value = tournamentData

    //bracket.value =  await bracketService.getBracket(id)
  //} catch (e) {
    //console.error(e)
    //error.value = true
  //} finally {
    //loading.value = false
  //}
//})

const handleBracketGenerated = () => {
  console.log('Bracket generated')

  /*
    later:
    refresh bracket from backend
  */
}

onMounted(() => {
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

    startDate: '2026-05-15 16:18',
    endDate: '2026-05-16 16:18',
    registrationCloseDate: '2026-05-10 16:18',

    sportId: '1',
    sportName: 'Chess',

    maxParticipants: 16,

    status: 'registration_closed',

    organizerId: '1',
    organizerName: 'Admin',

    backgroundImg:
      'https://images.steamusercontent.com/ugc/2492263902782461958/8A4E6A82E6B96C9E51B0E6A6A4C36E70A6AEB1A5/',

    participantsCount: 8,

    matches: [],
  }

  bracket.value = [
    {
      round: 1,

      roundDisplayName: '1/4',

      matchesCount: 2,
      notByeMatchesCount: 2,

      matches: [
        {
          matchId: '1',

          tournamentId: '1',

          round: 1,
          orderNumber: 1,

          player1Id: 'p1',
          player2Id: 'p2',

          status: 'completed',

          isBye: false,

          scorePlayer1: 2,
          scorePlayer2: 1,

          winnerId: 'p1',
        },

        {
          matchId: '2',

          tournamentId: '1',

          round: 1,
          orderNumber: 2,

          player1Id: 'p3',
          player2Id: 'p4',

          status: 'scheduled',

          isBye: false,

          scorePlayer1: null,
          scorePlayer2: null,

          winnerId: null,
        },
      ],
    },

    {
      round: 2,

      roundDisplayName: 'Final',

      matchesCount: 1,
      notByeMatchesCount: 1,

      matches: [
        {
          matchId: '3',

          tournamentId: '1',

          round: 2,
          orderNumber: 1,

          player1Id: 'p1',
          player2Id: null,

          status: 'pending',

          isBye: false,

          scorePlayer1: null,
          scorePlayer2: null,

          winnerId: null,
        },
      ],
    },
  ]

  loading.value = false
})
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
        @refresh-bracket="handleBracketGenerated"
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
        :rounds="bracket"
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