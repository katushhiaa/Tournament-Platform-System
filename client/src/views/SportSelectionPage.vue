<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import AppHeader from '../components/AppHeader.vue'
import { tournamentService } from '../services/tournamentService'
import { userService } from '../services/userService'
import type { IThemeOption } from '../types/Tournament'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const sports = ref<IThemeOption[]>([])
const selected = ref<Set<string>>(new Set())
const isSaving = ref(false)
const isSkipping = ref(false)
const saveError = ref<string | null>(null)
const skipError = ref<string | null>(null)
const showSkipModal = ref(false)
const loadError = ref(false)

onMounted(async () => {
  try {
    sports.value = await tournamentService.getSports()
  } catch {
    loadError.value = true
  }
})

const canSave = computed(() => selected.value.size > 0)

const toggleSport = (id: string) => {
  const next = new Set(selected.value)
  if (next.has(id)) {
    next.delete(id)
  } else {
    next.add(id)
  }
  selected.value = next
}

const finishOnboarding = () => {
  const uid = authStore.currentUser!.userId
  sessionStorage.removeItem(`new_user_${uid}`)
  localStorage.setItem(`onboarding_done_${uid}`, '1')
}

const handleSave = async () => {
  if (!canSave.value) return
  try {
    isSaving.value = true
    saveError.value = null
    await userService.savePreferences([...selected.value])

    const selectedNames = sports.value
      .filter(s => selected.value.has(s.id))
      .map(s => s.name.toLowerCase())
    localStorage.setItem(
      `preferences_${authStore.currentUser!.userId}`,
      JSON.stringify(selectedNames)
    )

    finishOnboarding()
    router.push(authStore.getDashboardRouteByRole(authStore.currentUser!.role))
  } catch (e: any) {
    saveError.value =
      e?.response?.data?.error?.message ?? 'Failed to save preferences. Please try again.'
  } finally {
    isSaving.value = false
  }
}

const handleSkipConfirm = async () => {
  try {
    isSkipping.value = true
    skipError.value = null
    await userService.completeOnboarding()
    showSkipModal.value = false

    finishOnboarding()
    router.push(authStore.getDashboardRouteByRole(authStore.currentUser!.role))
  } catch (e: any) {
    skipError.value =
      e?.response?.data?.error?.message ?? 'Failed to skip. Please try again.'
    showSkipModal.value = false
  } finally {
    isSkipping.value = false
  }
}
</script>

<template>
  <div class="page">
    <AppHeader />

    <main class="main">
      <div class="content">
        <div class="heading-row">
          <div>
            <h1 class="title">DEFINE YOUR PLAYSTYLE</h1>
            <p class="subtitle">Select your gaming interests</p>
          </div>
          <button class="skip-btn" @click="showSkipModal = true">
            Skip →
          </button>
        </div>

        <div v-if="loadError" class="error-text">
          Failed to load sports. Please refresh the page.
        </div>

        <div v-else class="grid">
          <button
            v-for="sport in sports"
            :key="sport.id"
            class="card"
            :class="{ 'card--selected': selected.has(sport.id) }"
            @click="toggleSport(sport.id)"
          >
            <img
              :src="sport.imageUrl ?? ''"
              :alt="sport.name"
              class="card__img"
            />
            <div class="card__overlay" />
            <span class="card__name">{{ sport.name.toUpperCase() }}</span>
            <span v-if="selected.has(sport.id)" class="card__check">✓</span>
          </button>
        </div>

        <p v-if="saveError" class="error-text">{{ saveError }}</p>
        <p v-if="skipError" class="error-text">{{ skipError }}</p>

        <button
          class="save-btn"
          :class="{ 'save-btn--active': canSave }"
          :disabled="!canSave || isSaving"
          @click="handleSave"
        >
          {{ isSaving ? 'Saving...' : canSave ? 'Lock it in!' : 'Select at least 1 skill to unlock continue' }}
        </button>
      </div>
    </main>

    <div v-if="showSkipModal" class="modal-overlay" @mousedown.self="showSkipModal = false">
      <div class="modal">
        <div class="modal__icon">
          <svg width="48" height="48" viewBox="0 0 48 48" fill="none">
            <circle cx="24" cy="24" r="22" stroke="#ff9800" stroke-width="2" />
            <path d="M24 14v14M24 32v2" stroke="#ff9800" stroke-width="2.5" stroke-linecap="round" />
          </svg>
        </div>
        <h2 class="modal__title">Confirmation</h2>
        <p class="modal__text">
          If you skip sport selection, tournaments on the home page<br />
          will be shown without considering your preferences.
        </p>
        <div class="modal__actions">
          <button
            class="modal__btn modal__btn--no"
            :disabled="isSkipping"
            @click="showSkipModal = false"
          >No</button>
          <button
            class="modal__btn modal__btn--yes"
            :disabled="isSkipping"
            @click="handleSkipConfirm"
          >{{ isSkipping ? '...' : 'Yes' }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page {
  min-height: 100vh;
  background: #060d1a;
  background-image: url('../assets/Background_view.png');
  background-size: cover;
  background-position: center;
  color: #fff;
}

.main {
  padding: 40px 80px 80px;
}

.content {
  max-width: 1200px;
  margin: 0 auto;
}

.heading-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 40px;
  margin-top: 80px;
}

.title {
  font-size: 36px;
  font-weight: 900;
  color: #ff9800;
  letter-spacing: 0.04em;
  margin: 0 0 8px;
}

.subtitle {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.6);
  margin: 0;
}

.skip-btn {
  background: transparent;
  border: 1px solid rgba(255, 255, 255, 0.4);
  border-radius: 8px;
  color: rgba(255, 255, 255, 0.7);
  font-size: 14px;
  padding: 8px 20px;
  cursor: pointer;
  transition: border-color 0.2s, color 0.2s;
  white-space: nowrap;
}

.skip-btn:hover {
  border-color: #fff;
  color: #fff;
}

.grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 16px;
  margin-bottom: 32px;
}

.card {
  position: relative;
  aspect-ratio: 1 / 1.15;
  border-radius: 12px;
  overflow: hidden;
  border: 2px solid transparent;
  cursor: pointer;
  padding: 0;
  background: #111;
  transition: border-color 0.2s, transform 0.15s;
}

.card:hover {
  transform: scale(1.03);
}

.card--selected {
  border-color: #ff9800;
}

.card__img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.card__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    160deg,
    rgba(180, 140, 0, 0.3) 0%,
    rgba(0, 0, 0, 0.55) 100%
  );
}

.card__name {
  position: absolute;
  bottom: 10px;
  left: 0;
  right: 0;
  text-align: center;
  font-size: 13px;
  font-weight: 800;
  color: #fff;
  letter-spacing: 0.07em;
  text-shadow: 0 1px 4px rgba(0, 0, 0, 0.8);
}

.card__check {
  position: absolute;
  top: 8px;
  right: 10px;
  font-size: 16px;
  color: #ff9800;
  font-weight: 900;
  text-shadow: 0 0 6px rgba(0, 0, 0, 0.8);
}

.save-btn {
  width: 100%;
  height: 52px;
  border-radius: 12px;
  border: none;
  background: rgba(255, 152, 0, 0.25);
  color: rgba(255, 255, 255, 0.5);
  font-size: 16px;
  font-weight: 700;
  cursor: not-allowed;
  transition: background 0.2s, color 0.2s;
}

.save-btn--active {
  background: #ff9800;
  color: #fff;
  cursor: pointer;
}

.save-btn--active:hover:not(:disabled) {
  opacity: 0.9;
}

.save-btn:disabled:not(.save-btn--active) {
  opacity: 1;
}

.error-text {
  color: #ce0f0f;
  font-size: 14px;
  margin-bottom: 12px;
  text-align: center;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  z-index: 1000;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal {
  background: #1a2230;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 20px;
  padding: 48px 56px;
  max-width: 480px;
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
  text-align: center;
}

.modal__icon {
  margin-bottom: 4px;
}

.modal__title {
  font-size: 28px;
  font-weight: 700;
  color: #fff;
  margin: 0;
}

.modal__text {
  font-size: 14px;
  color: rgba(255, 255, 255, 0.6);
  line-height: 1.6;
  margin: 0;
}

.modal__actions {
  display: flex;
  gap: 16px;
  margin-top: 8px;
}

.modal__btn {
  width: 120px;
  height: 44px;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.2s;
}

.modal__btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.modal__btn--no {
  border: 1px solid rgba(255, 255, 255, 0.3);
  background: transparent;
  color: rgba(255, 255, 255, 0.7);
}

.modal__btn--yes {
  border: none;
  background: #ff9800;
  color: #fff;
}

.modal__btn--yes:hover:not(:disabled) {
  opacity: 0.88;
}

@media (max-width: 900px) {
  .main { padding: 24px 16px 60px; }
  .grid { grid-template-columns: repeat(3, 1fr); }
  .heading-row { flex-direction: column; gap: 16px; }
}

@media (max-width: 600px) {
  .grid { grid-template-columns: repeat(2, 1fr); }
}
</style>