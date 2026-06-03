<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

import { tournamentService } from '../services/tournamentService'
import { participationService } from '../services/participationService'
import { bracketService } from '../services/bracketService'

import type { ITournament } from '../types/Tournament'
import type { Participant } from '../types/Participant'
import type { IBracketStructure } from '../types/Bracket'

import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import TournamentHeader from '../components/tournament/TournamentHeader.vue'
import TournamentTabs from '../components/tournament/TournamentTabs.vue'
import TournamentOverview from '../components/tournament/TournamentOverview.vue'
import TournamentParticipants from '../components/tournament/TournamentParticipants.vue'
import TournamentBracket from '../components/tournament/TournamentBracket.vue'
import TournamentOtherEvents from '../components/tournament/TournamentOtherEvents.vue'
import TournamentSkeleton from '../components/tournament/TournamentSkeleton.vue'
import TournamentError from '../components/tournament/TournamentError.vue'

import AppToast from '../components/ui/AppToast.vue'

const route = useRoute()
const authStore = useAuthStore()

const tournament = ref<ITournament | null>(null)
const participants = ref<Participant[]>([])
const bracket = ref<IBracketStructure>([])
const toast = ref('')
const loading = ref(true)
const error = ref(false)
const activeTab = ref('overview')

const showToast = (msg: string) => {
  toast.value = msg
  setTimeout(() => { toast.value = '' }, 4000)
}

const loadData = async () => {
  try {
    loading.value = true
    error.value = false
    const id = route.params.id as string
    const [tournamentData, participantsData, bracketData] = await Promise.all([
      tournamentService.getTournamentById(id),
      participationService.getTournamentParticipants(id),
      bracketService.getBracket(id),
    ])
    tournament.value = tournamentData
    participants.value = participantsData
    bracket.value = bracketData
    const joinToast = sessionStorage.getItem('joinToast')
    if (joinToast) {
      showToast(joinToast)
      sessionStorage.removeItem('joinToast')
    }
  } catch (e) {
    console.error(e)
    error.value = true
  } finally {
    loading.value = false
  }
}

onMounted(loadData)

const isBracketLoading = ref(false)
const bracketError = ref('')

const handleBracketGenerated = async () => {
  const id = route.params.id as string
  try {
    isBracketLoading.value = true
    bracketError.value = ''
    const [updatedTournament, updatedBracket] = await Promise.all([
      tournamentService.getTournamentById(id),
      bracketService.getBracket(id),
    ])
    tournament.value = updatedTournament
    bracket.value = updatedBracket
    activeTab.value = 'grid'
    showToast('Bracket generated successfully!')
  } catch (e) {
    console.error('Failed to refresh after bracket generation', e)
    bracketError.value = 'Failed to update bracket. Please refresh the page.'
    setTimeout(() => { bracketError.value = '' }, 5000)
  } finally {
    isBracketLoading.value = false
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
      <TournamentError v-else-if="error" @retry="loadData" />

      <template v-else-if="tournament">
        <TournamentHeader
          :tournament="tournament"
          :participants="participants"
          :current-user="authStore.currentUser
            ? { id: authStore.currentUser.userId, role: authStore.currentUser.role }
            : null"
          @refresh-bracket="handleBracketGenerated"
          @refresh-participants="handleRefreshParticipants"
          @show-toast="showToast"
        />

        <TournamentTabs
          :active-tab="activeTab"
          @change="activeTab = $event"
        />

        <AppToast v-if="bracketError" :message="bracketError" type="error" />
        <AppToast v-if="toast" :message="toast" type="success" />

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
          :is-organizer="authStore.currentUser?.userId === tournament.organizerId"
          :tournament-id="tournament.id"
          :is-loading="isBracketLoading"
          @bracket-updated="handleBracketGenerated"
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

.vt-toast {
  position: fixed;
  bottom: 32px;
  left: 50%;
  transform: translateX(-50%);
  background: #84c082;
  color: #151d22;
  padding: 14px 28px;
  border-radius: 12px;
  font-size: 15px;
  font-weight: 600;
  z-index: 999;
}

.vt-toast--error {
  background: #ce0f0f;
  color: #fff;
}

@media (max-width: 768px) {
  .page {
    padding: 16px;
  }
}
</style>