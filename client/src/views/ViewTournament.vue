<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'

import { tournamentService } from '../services/tournamentService'
import { participationService } from '../services/participationService'
import { bracketService } from '../services/bracketService'

import type { ITournament } from '../types/Tournament'
import type { Participant } from '../types/Participant'
import type { IBracketStructure } from '../types/Bracket'

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

const route = useRoute()

const tournament = ref<ITournament | null>(null)
const participants = ref<Participant[]>([])
const bracket = ref<IBracketStructure>([])

const loading = ref(true)
const error = ref(false)

const activeTab = ref('overview')

import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()

onMounted(async () => {
  try {
    loading.value = true

    const id = route.params.id as string

    const [tournamentData, participantsData, bracketData] =
      await Promise.all([
        tournamentService.getTournamentById(id),
        participationService.getTournamentParticipants(id),
        bracketService.getBracket(id),
      ])

    tournament.value = tournamentData
    participants.value = participantsData
    bracket.value = bracketData

  } catch (e) {
    console.error(e)
    error.value = true
  } finally {
    loading.value = false
  }
})

const handleBracketGenerated = async () => {
  const id = route.params.id as string
  try {
    const [updatedTournament, updatedBracket] = await Promise.all([
      tournamentService.getTournamentById(id),
      bracketService.getBracket(id),
    ])
    tournament.value = updatedTournament
    bracket.value = updatedBracket
    activeTab.value = 'grid'
  } catch (e) {
    console.error('Failed to refresh after bracket generation', e)
  }
}

const handleRefreshParticipants = async () => {
  const id = route.params.id as string
  const [updatedParticipants, updatedTournament] = await Promise.all([
    participationService.getTournamentParticipants(id),
    tournamentService.getTournamentById(id),
  ])
  participants.value = updatedParticipants
  tournament.value = updatedTournament
}
</script>

<template>
  <div class="page">
    <AppHeader />

    <main>
      <TournamentSkeleton v-if="loading" />
      <TournamentError v-else-if="error" />

      <template v-else-if="tournament">
        <TournamentHeader
          :tournament="tournament"
          :participants="participants"
          :current-user="authStore.currentUser
            ? { id: authStore.currentUser.userId, role: authStore.currentUser.role }
            : null"
          @refresh-bracket="handleBracketGenerated"
          @refresh-participants="handleRefreshParticipants"
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
          :participants="participants"
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
  </div>
</template>


<style scoped>
.page {
  min-height: 100vh;

  background-image: url('../assets/Background_view.png');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

@media (max-width: 768px) {
  .page {
    padding: 16px;
  }
}
</style>