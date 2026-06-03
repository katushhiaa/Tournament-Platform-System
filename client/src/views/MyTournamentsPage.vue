<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import TournamentCard from '../components/TournamentCard.vue'
import { tournamentService } from '../services/tournamentService'
import { useAuthStore } from '../stores/authStore'
import type { ITournamentPreview } from '../types/Tournament'
import defaultCardBg from '../assets/hero-card.jpg'

const authStore = useAuthStore()
const isOrganizer = computed(() => authStore.currentUser?.role === 'organizer')

const allTournaments = ref<ITournamentPreview[]>([])
const loading = ref(true)
const error = ref(false)
const searchQuery = ref('')
const currentPage = ref(1)
const PAGE_SIZE = 15

const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  })

const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString('uk-UA', {
    hour: '2-digit', minute: '2-digit',
  })

onMounted(async () => {
  if (!authStore.currentUser) return

  try {
    allTournaments.value = await tournamentService.getUserTournaments({
      userId: authStore.currentUser.userId,
      pageSize: 100,
    })
  } catch {
    error.value = true
  } finally {
    loading.value = false
  }
})

const filtered = computed(() => {
  const q = searchQuery.value.trim().toLowerCase()
  if (!q) return allTournaments.value
  return allTournaments.value.filter(t =>
    t.title.toLowerCase().includes(q) ||
    t.sportName?.toLowerCase().includes(q)
  )
})

watch(searchQuery, () => { currentPage.value = 1 })

const totalPages = computed(() =>
  Math.max(1, Math.ceil(filtered.value.length / PAGE_SIZE))
)

const paginated = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE
  return filtered.value.slice(start, start + PAGE_SIZE)
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

    <main class="main">
      <div class="title-row">
        <h1 class="title">My Tournaments</h1>
        <router-link
          v-if="isOrganizer"
          to="/tournaments/create"
          class="create-btn"
        >
          + Create Tournament
        </router-link>
      </div>

      <div class="search-wrap">
        <svg class="search-icon" width="20" height="20" viewBox="0 0 24 24" fill="none">
          <circle cx="11" cy="11" r="7" stroke="rgba(255,255,255,0.45)" stroke-width="2" />
          <path d="M16.5 16.5L21 21" stroke="rgba(255,255,255,0.45)" stroke-width="2" stroke-linecap="round" />
        </svg>
        <input
          v-model="searchQuery"
          class="search-input"
          placeholder="Search tournaments, games, or keywords..."
        />
      </div>

      <template v-if="loading">
        <div class="grid">
          <div v-for="n in 4" :key="n" class="skeleton" />
        </div>
      </template>

      <p v-else-if="error" class="empty">Failed to load tournaments.</p>
      <p v-else-if="!paginated.length" class="empty">
        {{ searchQuery ? 'No tournaments match your search.' : 'You have no tournaments yet.' }}
      </p>

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

.main {
  padding: 120px 80px 100px;
}

.title-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 32px;
}

.title {
  font-size: 40px;
  font-weight: 700;
  margin: 0;
}

.create-btn {
  display: inline-flex;
  align-items: center;
  height: 44px;
  padding: 0 24px;
  border-radius: 10px;
  background: #ff9800;
  color: #fff;
  font-size: 15px;
  font-weight: 700;
  text-decoration: none;
  transition: opacity 0.2s;
  white-space: nowrap;
}

.create-btn:hover {
  opacity: 0.88;
}

.search-wrap {
  position: relative;
  max-width: 600px;
  margin: 0 0 40px;
}

.search-icon {
  position: absolute;
  left: 16px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
}

.search-input {
  width: 100%;
  height: 48px;
  border-radius: 10px;
  border: 1px solid rgba(255, 255, 255, 0.15);
  background: rgba(255, 255, 255, 0.06);
  color: #fff;
  font-size: 15px;
  padding: 0 20px 0 48px;
  outline: none;
  box-sizing: border-box;
  transition: border-color 0.2s;
}

.search-input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.search-input:focus {
  border-color: rgba(255, 255, 255, 0.4);
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
  color: rgba(255, 255, 255, 0.6);
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
  border: 1px solid rgba(255, 255, 255, 0.2);
  background: transparent;
  color: #fff;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s, border-color 0.2s;
}

.page-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
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

@media (max-width: 768px) {
  .main {
    padding: 100px 16px 80px;
  }

  .title-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }

  .title {
    font-size: 28px;
  }

  .search-wrap {
    max-width: 100%;
  }
}
</style>