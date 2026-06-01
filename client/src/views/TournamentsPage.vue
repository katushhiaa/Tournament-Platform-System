<script setup lang="ts">
import { onMounted, ref } from 'vue'
import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import TournamentCard from '../components/TournamentCard.vue'
import { tournamentService } from '../services/tournamentService'
import type { ITournamentPreview } from '../types/Tournament'
import defaultCardBg from '../assets/hero-card.jpg'
import { useAuthStore } from '../stores/authStore'
import { getSportPreferences, sortByPreferences } from '../utils/sortByPreferences'

const authStore = useAuthStore()
const tournaments = ref<ITournamentPreview[]>([])
const loading = ref(true)
const error = ref(false)

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  })

const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString('uk-UA', {
    hour: '2-digit', minute: '2-digit',
  })

onMounted(async () => {
  try {
    const raw = await tournamentService.getTournaments({ pageSize: 20 })
    const prefs = authStore.currentUser
      ? getSportPreferences(authStore.currentUser.userId)
      : []
    tournaments.value = sortByPreferences(raw, prefs)
  } catch (e) {
    error.value = true
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="page">
    <AppHeader />

    <main class="main">
      <h1 class="title">All Tournaments</h1>

      <div class="grid">
        <template v-if="loading">
          <div v-for="n in 8" :key="n" class="skeleton" />
        </template>

        <p v-else-if="error" class="empty">Failed to load tournaments.</p>
        <p v-else-if="!tournaments.length" class="empty">No tournaments found.</p>

        <TournamentCard
          v-else
          v-for="t in tournaments"
          :key="t.id"
          :id="t.id"
          :image="t.backgroundImg ?? defaultCardBg"
          :title="t.title"
          :type="t.sportName"
          :date="formatDate(t.startDate)"
          :time="formatTime(t.startDate)"
          :participants="`${t.participantsCount}/${t.maxParticipants}`"
          :status="t.status"
        />
      </div>
    </main>

    <SiteFooter />
  </div>
</template>

<style scoped>
.page {
  min-height: 100vh;
  background: #151d22;
  color: #fffcf2;
}

.main {
  padding: 120px 80px 100px;
}

.title {
  font-size: 40px;
  font-weight: 700;
  margin-bottom: 48px;
  text-align: center;
}

.grid {
  display: flex;
  flex-wrap: wrap;
  gap: 24px;
  justify-content: center;
}

.skeleton {
  width: 230px;
  min-height: 306px;
  border-radius: 18px;
  background: linear-gradient(90deg, #2e3a42 25%, #3a4a54 50%, #2e3a42 75%);
  background-size: 200% 100%;
  animation: shimmer 1.4s infinite;
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

.empty {
  text-align: center;
  color: rgba(255,255,255,0.6);
  font-size: 16px;
}

@media (max-width: 768px) {
  .main {
    padding: 100px 16px 80px;
  }
}
</style>