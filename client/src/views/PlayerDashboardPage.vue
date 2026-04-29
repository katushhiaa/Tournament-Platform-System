<script setup lang="ts">
import { computed } from 'vue';
import AppHeader from '../components/AppHeader.vue';
import TournamentCard from '../components/TournamentCard.vue';
import SiteFooter from '../components/SiteFooter.vue';
import { useAuthStore } from '../stores/authStore';

import dashboardBg from '../assets/hero-bg.png';
import cs2Card from '../assets/cs2-card.png';
import tennisCard from '../assets/tennis-card.png';
import forzaCard from '../assets/forza-card.png';
import chessCard from '../assets/chess-card.png';

const authStore = useAuthStore();
const userName = computed(() => {
  const fullName = authStore.currentUser?.fullName?.trim() || '';

  if (!fullName) return 'User';

  const parts = fullName.split(/\s+/);

  if (parts.length >= 2) {
    return parts[1];
  }

  return parts[0];
});
</script>

<template>
  <div class="dashboard">
    <section class="dashboard-hero" :style="{ backgroundImage: `url(${dashboardBg})` }">
      <div class="dashboard-hero__overlay"></div>

      <AppHeader />

      <div class="dashboard-hero__content">
        <h1 class="dashboard-hero__title">Hi, {{ userName }}!</h1>
        <p class="dashboard-hero__subtitle">
          Welcome back! Explore tournaments or track your progress.
        </p>
      </div>
    </section>

    <section class="dashboard-section dashboard-section--active">
      <div class="dashboard-section__inner">
        <h2 class="dashboard-section__title">Active tournaments</h2>
        <p class="dashboard-section__subtitle">
          Tournaments you're currently playing in
        </p>

        <div class="dashboard-grid">
          <TournamentCard
            :image="cs2Card"
            title="Counter strike 2"
            type="Game"
            date="24.05.2026"
            time="18:00"
            participants="32/64"
          />
          <TournamentCard
            :image="tennisCard"
            title="Tennis tour"
            type="Tennis"
            date="28.06.2026"
            time="16:00"
            participants="6/6"
          />
          <TournamentCard
            :image="forzaCard"
            title="Forza horizon 5"
            type="Game"
            date="21.03.2026"
            time="20:00"
            participants="16/16"
          />
          <TournamentCard
            :image="chessCard"
            title="Chess tour"
            type="Chess"
            date="23.02.2026"
            time="15:00"
            participants="12/12"
          />
        </div>

        <div class="dashboard-actions dashboard-actions--single">
          <button class="main-button">
            <span>View All Tournaments</span>
          </button>
        </div>
      </div>
    </section>

    <section class="dashboard-section dashboard-section--my">
      <div class="dashboard-section__inner">
        <h2 class="dashboard-section__title">My tournaments</h2>
        <p class="dashboard-section__subtitle">Your tournaments</p>

        <div class="dashboard-grid">
          <TournamentCard
            :image="chessCard"
            title="Chess tour"
            type="Chess"
            date="23.02.2026"
            time="15:00"
            participants="12/12"
          />
          <TournamentCard
            :image="tennisCard"
            title="Tennis tour"
            type="Tennis"
            date="28.06.2026"
            time="16:00"
            participants="6/6"
          />
          <TournamentCard
            :image="cs2Card"
            title="Counter strike 2"
            type="Game"
            date="24.05.2026"
            time="18:00"
            participants="32/64"
          />
          <TournamentCard
            :image="forzaCard"
            title="Forza horizon 5"
            type="Game"
            date="21.03.2026"
            time="20:00"
            participants="16/16"
          />
        </div>

        <div class="dashboard-actions dashboard-actions--single">
          <button class="main-button">
            <span>View All Tournaments</span>
          </button>
        </div>
      </div>
    </section>

    <SiteFooter />
  </div>
</template>

<style scoped>
.dashboard {
  background: #252e35;
  color: #fffcf2;
}

.dashboard-hero {
  position: relative;
  min-height: 420px;
  background-size: cover;
  background-position: center;
}

.dashboard-hero__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(37, 46, 53, 0.3),
    rgba(37, 46, 53, 0.7)
  );
}

.dashboard-hero__content {
  position: relative;
  z-index: 1;
  padding-top: 140px;
  text-align: center;
}

.dashboard-hero__title {
  margin: 0 0 15px;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.1;
}

.dashboard-hero__subtitle {
  margin: 0;
  font-size: 16px;
  line-height: 1.4;
}

.dashboard-section {
  padding: 80px 0;
}

.dashboard-section--active {
  background: #3a444c;
}

.dashboard-section--my {
  background: #252e35;
  padding-top: 100px;
  padding-bottom: 159px;
}

.dashboard-section__inner {
  width: min(1224px, calc(100% - 64px));
  margin: 0 auto;
}

.dashboard-section__title {
  margin: 0 0 20px;
  text-align: center;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.1;
}

.dashboard-section__subtitle {
  margin: 0 0 40px;
  text-align: center;
  font-size: 16px;
  line-height: 1.4;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(4, 230px);
  justify-content: center;
  gap: 32px;
}

.dashboard-actions {
  display: flex;
  justify-content: center;
  margin-top: 60px;
}

.main-button {
  width: 280px;
  height: 46px;
  border-radius: 12px;
  border: 1px solid #ff9800;
  background: #ff9800;
  color: #fffcf2;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition:
    background 0.2s ease,
    color 0.2s ease,
    border-color 0.2s ease;
}

.main-button:hover {
  background: #ff9800;
  color: #fffcf2;
}

.main-button:active,
.main-button:focus-visible {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
  outline: none;
}

@media (max-width: 1200px) {
  .dashboard-grid {
    grid-template-columns: repeat(2, 230px);
  }
}

@media (max-width: 960px) {
  .dashboard-hero__content {
    padding-top: 120px;
    padding-left: 20px;
    padding-right: 20px;
  }

  .dashboard-hero__title {
    font-size: 40px;
  }

  .dashboard-section__inner {
    width: min(100%, calc(100% - 32px));
  }

  .dashboard-section__title {
    font-size: 40px;
  }

  .dashboard-section--my {
    padding-bottom: 100px;
  }
}

@media (max-width: 640px) {
  .dashboard-hero__content {
    padding-top: 100px;
    padding-left: 16px;
    padding-right: 16px;
  }

  .dashboard-hero__title {
    font-size: 32px;
  }

  .dashboard-hero__subtitle {
    font-size: 15px;
  }

  .dashboard-section__title {
    font-size: 32px;
  }

  .dashboard-grid {
    grid-template-columns: 1fr;
    justify-items: center;
  }

  .main-button {
    width: 100%;
    max-width: 320px;
  }
}
</style>