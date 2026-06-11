<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import TournamentCard from '../components/TournamentCard.vue'
import { tournamentService } from '../services/tournamentService'
import { useAuthStore } from '../stores/authStore'
import type { ITournamentPreview } from '../types/Tournament'
import defaultCardBg from '../assets/hero-card.png'
import heroBg from '../assets/Background_2.png'

const authStore = useAuthStore()
const isOrganizer = computed(() => authStore.currentUser?.role === 'organizer')

const allTournaments = ref<ITournamentPreview[]>([])
const loading = ref(true)
const error = ref(false)
const searchQuery = ref('')
const currentPage = ref(1)
const PAGE_SIZE = 12


const formatDate = (iso: string) =>
  new Date(iso).toLocaleDateString('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
  })

const formatTime = (iso: string) =>
  new Date(iso).toLocaleTimeString('uk-UA', {
    hour: '2-digit', minute: '2-digit',
  })

let searchTimeout: ReturnType<typeof setTimeout> | null = null

async function fetchTournaments(q?: string) {
  if (!authStore.currentUser) return
  loading.value = true
  error.value = false
  try {
    allTournaments.value = await tournamentService.getUserTournaments({
      userId: authStore.currentUser.userId,
      pageSize: 100,
      q: q || undefined,
    })
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

   
    <section class="hero" :style="{ backgroundImage: `url(${heroBg})` }">
      <div class="hero__overlay" />
      <div class="hero__content">
        <h1 class="hero__title">My tournaments</h1>
       
        <p v-if="!isOrganizer" class="hero__subtitle">
          Find and join the best esports tournaments
        </p>

        
        <div class="hero__search-row">
          <div class="search-wrap" :class="{ 'search-wrap--full': !isOrganizer }">
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

          
          <router-link
            v-if="isOrganizer"
            to="/tournaments/create"
            class="create-btn"
          >
            <span class="create-btn__icon">+</span>
            Create tournament
          </router-link>
        </div>
      </div>
    </section>

   
    <main class="main">
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


.hero {
  position: relative;
  background-size: cover;
  background-position: center top;
  padding: 100px 80px 60px;
  min-height: 300px;
}

.hero__content {
  position: relative;
  z-index: 1;
  max-width: 1200px;
  margin: 0 auto;
}

.hero__title {
  font-size: 48px;
  font-weight: 700;
  margin: 0 0 8px;
  color: #fff;
}

.hero__subtitle {
  font-size: 16px;
  color: rgba(255, 255, 255, 0.7);
  margin: 0 0 32px;
}


.hero__title + .hero__search-row {
  margin-top: 32px;
}

.hero__search-row {
  display: flex;
  align-items: center;
  gap: 16px;
}


.search-wrap {
  position: relative;
  flex: 1;
  max-width: 600px;
}

.search-wrap--full {
  max-width: 600px;
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
  height: 52px;
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
  border-color: rgba(255, 255, 255, 0.35);
}


.create-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  height: 52px;
  padding: 0 28px;
  border-radius: 10px;
  background: #ff9800;
  color: #fff;
  font-size: 15px;
  font-weight: 700;
  text-decoration: none;
  white-space: nowrap;
  transition: opacity 0.2s;
  flex-shrink: 0;
}

.create-btn:hover {
  opacity: 0.88;
}

.create-btn__icon {
  font-size: 20px;
  line-height: 1;
  margin-top: -1px;
}


.main {
  padding: 48px 80px 100px;
  max-width: 1360px;
  margin: 0 auto;
  box-sizing: border-box;
  width: 100%;
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
  color: rgba(255, 255, 255, 0.6);
  font-size: 16px;
  padding: 60px 0;
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


@media (max-width: 1200px) {
  .grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 900px) {
  .hero {
    padding: 90px 32px 48px;
  }

  .main {
    padding: 40px 32px 80px;
  }

  .grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .hero {
    padding: 80px 16px 40px;
  }

  .hero__title {
    font-size: 32px;
  }

  .hero__search-row {
    flex-direction: column;
    align-items: stretch;
  }

  .search-wrap,
  .search-wrap--full {
    max-width: 100%;
  }

  .create-btn {
    justify-content: center;
    width: 100%;
  }

  .main {
    padding: 32px 16px 80px;
  }

  }
</style>