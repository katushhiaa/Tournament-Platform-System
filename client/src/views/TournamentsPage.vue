<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import TournamentCard from '../components/TournamentCard.vue'
import { tournamentService } from '../services/tournamentService'
import type { ITournamentPreview } from '../types/Tournament'
import defaultCardBg from '../assets/hero-card.png'
import tournamentsBg from '../assets/hero-bg.png'

const allTournaments = ref<ITournamentPreview[]>([])
const loading = ref(true)
const error = ref(false)
const isFallback = ref(false)
const searchQuery = ref('')
const currentPage = ref(1)
const PAGE_SIZE = 12

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', { day: '2-digit', month: '2-digit', year: 'numeric' })

const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' })

let searchTimeout: ReturnType<typeof setTimeout> | null = null

async function fetchTournaments(q?: string) {
  loading.value = true
  error.value = false
  try {
    const res = await tournamentService.getTournaments({ pageSize: 100, q: q || undefined })
    allTournaments.value = res.tournaments
    isFallback.value = res.fallback_reason === 'NO_MATCHING_TOURNAMENTS'
  } catch {
    error.value = true
  } finally {
    loading.value = false
  }
}

onMounted(() => fetchTournaments())

watch(searchQuery, (val) => {
  currentPage.value = 1
  if (searchTimeout) clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => fetchTournaments(val.trim()), 350)
})

const totalPages = computed(() =>
  Math.max(1, Math.ceil(allTournaments.value.length / PAGE_SIZE))
)

const paginated = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE
  return allTournaments.value.slice(start, start + PAGE_SIZE)
})

const visiblePages = computed(() => {
  const total = totalPages.value
  const cur = currentPage.value
  if (total <= 5) return Array.from({ length: total }, (_, i) => i + 1)
  if (cur <= 3) return [1, 2, 3, 4, 5]
  if (cur >= total - 2) return [total - 4, total - 3, total - 2, total - 1, total]
  return [cur - 2, cur - 1, cur, cur + 1, cur + 2]
})
</script>

<template>
  <div class="page">
    <AppHeader />

    <div class="hero" :style="{ backgroundImage: `url(${tournamentsBg})` }">
      <div class="hero__overlay" />
      <div class="hero__content">
        <h1 class="hero__title">Tournaments</h1>
        <p class="hero__subtitle">Find and join the best esports tournaments</p>
        <div class="search-wrap">
          <svg class="search-icon" width="20" height="20" viewBox="0 0 24 24" fill="none">
            <circle cx="11" cy="11" r="7" stroke="rgba(255,255,255,0.5)" stroke-width="2"/>
            <path d="M16.5 16.5L21 21" stroke="rgba(255,255,255,0.5)" stroke-width="2" stroke-linecap="round"/>
          </svg>
          <input
            v-model="searchQuery"
            class="search-input"
            placeholder="Search tournaments, games, or keywords..."
          />
        </div>
      </div>
    </div>

    <main class="main">
      <p v-if="isFallback" class="fallback-msg">
        No tournaments found for your preferred sports. Showing all available tournaments.
      </p>

      <template v-if="loading">
        <div class="grid">
          <div v-for="n in 12" :key="n" class="skeleton" />
        </div>
      </template>

      <p v-else-if="error" class="empty">Failed to load tournaments.</p>
      <p v-else-if="!paginated.length" class="empty">No tournaments found.</p>

      <div v-else class="grid">
        <TournamentCard
          v-for="t in paginated"
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

      <div v-if="totalPages > 1" class="pagination">
        <button
          class="page-btn page-btn--arrow"
          :disabled="currentPage === 1"
          @click="currentPage--"
        >‹</button>

        <button
          v-for="p in visiblePages"
          :key="p"
          class="page-btn"
          :class="{ 'page-btn--active': p === currentPage }"
          @click="currentPage = p"
        >{{ p }}</button>

        <button
          class="page-btn page-btn--arrow"
          :disabled="currentPage === totalPages"
          @click="currentPage++"
        >›</button>
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

.hero {
  position: relative;
  padding: 120px 24px 60px;
  background-size: cover;
  background-position: center;
  text-align: center;
}

.hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(21,49,206,0.55) 0%, rgba(10,15,40,0.75) 100%);
}

.hero__content {
  position: relative;
  z-index: 1;
  max-width: 700px;
  margin: 0 auto;
}

.hero__title {
  font-size: 48px;
  font-weight: 800;
  margin: 0 0 12px;
}

.hero__subtitle {
  font-size: 16px;
  opacity: 0.75;
  margin: 0 0 32px;
}

.search-wrap {
  position: relative;
  max-width: 600px;
  margin: 0 auto;
}

.search-icon {
  position: absolute;
  left: 18px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
}

.search-input {
  width: 100%;
  height: 52px;
  border-radius: 12px;
  border: 1px solid rgba(255,255,255,0.2);
  background: rgba(255,255,255,0.08);
  color: #fff;
  font-size: 15px;
  padding: 0 20px 0 50px;
  outline: none;
  box-sizing: border-box;
  transition: border-color 0.2s;
}

.search-input::placeholder { color: rgba(255,255,255,0.45); }
.search-input:focus { border-color: rgba(255,255,255,0.5); }

.main {
  padding: 48px 80px 80px;
}

.fallback-msg {
  text-align: center;
  color: rgba(255, 255, 255, 0.55);
  font-size: 14px;
  margin-bottom: 24px;
}

.grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  column-gap: 32px;
  row-gap: 100px;
}

.skeleton {
  width: 100%;
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

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin-top: 48px;
}

.page-btn {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  border: 1px solid rgba(255,255,255,0.2);
  background: transparent;
  color: #fff;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s, border-color 0.2s;
}

.page-btn:hover:not(:disabled) {
  background: rgba(255,255,255,0.1);
}

.page-btn--active {
  background: #1531ce;
  border-color: #1531ce;
}

.page-btn--arrow {
  font-size: 20px;
}

.page-btn:disabled {
  opacity: 0.35;
  cursor: not-allowed;
}

@media (max-width: 1200px) {
  .grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 900px) {
  .grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .main { padding: 32px 16px 60px; }
  .hero__title { font-size: 32px; }
  .grid {
    grid-template-columns: 1fr;
  }
}
</style>