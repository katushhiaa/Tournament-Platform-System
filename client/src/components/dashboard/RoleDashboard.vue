<template>
  <div class="role-dashboard">
    <section class="role-dashboard__hero" :style="{ backgroundImage: `url(${heroBg})` }">
      <div class="role-dashboard__overlay"></div>

      <AppHeader />

      <div class="role-dashboard__hero-content">
        <h1 class="role-dashboard__title">Hi, {{ firstName }}!</h1>
        <p class="role-dashboard__subtitle">{{ heroSubtitle }}</p>
      </div>
    </section>

    <main class="role-dashboard__main">
      <section class="dashboard-section dashboard-section--active">
        <div class="dashboard-section__inner">
          <h2 class="dashboard-section__title">Active tournaments</h2>
          <p class="dashboard-section__subtitle">{{ activeSubtitle }}</p>

          <div class="dashboard-section__grid">
            <TournamentCard
              v-for="item in activeTournaments"
              :key="item.title"
              :image="item.image"
              :title="item.title"
              :type="item.type"
              :date="item.date"
              :time="item.time"
              :participants="item.participants"
            />
          </div>

          <div class="dashboard-section__actions">
            <button class="dashboard-button dashboard-button--blue">
              <span class="dashboard-button__icon">☰</span>
              <span>View All Tournaments</span>
            </button>

            <button
              v-if="showCreateButton"
              class="dashboard-button dashboard-button--orange"
            >
              <span class="dashboard-button__icon">⊕</span>
              <span>Create tournament</span>
            </button>
          </div>
        </div>
      </section>

      <section class="dashboard-section dashboard-section--my">
        <div class="dashboard-section__inner">
          <h2 class="dashboard-section__title">My tournaments</h2>
          <p class="dashboard-section__subtitle">{{ mySubtitle }}</p>

          <div class="dashboard-section__grid">
            <TournamentCard
              v-for="item in myTournaments"
              :key="`my-${item.title}`"
              :image="item.image"
              :title="item.title"
              :type="item.type"
              :date="item.date"
              :time="item.time"
              :participants="item.participants"
            />
          </div>

          <div class="dashboard-section__actions dashboard-section__actions--single">
            <button class="dashboard-button dashboard-button--blue">
              <span class="dashboard-button__icon">☰</span>
              <span>View All Tournaments</span>
            </button>
          </div>
        </div>
      </section>
    </main>

    <SiteFooter />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import AppHeader from '../AppHeader.vue';
import TournamentCard from '../TournamentCard.vue';
import SiteFooter from '../SiteFooter.vue';
import { authStore } from '../../state/authStore';

import heroBg from '../../assets/hero-bg.png';
import cs2Card from '../../assets/cs2-card.png';
import tennisCard from '../../assets/tennis-card.png';
import forzaCard from '../../assets/forza-card.png';
import chessCard from '../../assets/chess-card.png';

defineProps<{
  heroSubtitle: string;
  activeSubtitle: string;
  mySubtitle: string;
  showCreateButton: boolean;
}>();

const firstName = computed(() => {
  const fullName = authStore.state.user?.fullName?.trim();
  if (!fullName) return 'Name';
  return fullName.split(' ')[0];
});

const activeTournaments = [
  {
    image: cs2Card,
    title: 'Counter strike 2',
    type: 'Game',
    date: '24.05.2026',
    time: '18:00',
    participants: '32/64',
  },
  {
    image: tennisCard,
    title: 'Tennis tour',
    type: 'Tennis',
    date: '28.06.2026',
    time: '16:00',
    participants: '6/6',
  },
  {
    image: forzaCard,
    title: 'Forza horizon 5',
    type: 'Game',
    date: '21.03.2026',
    time: '20:00',
    participants: '16/16',
  },
  {
    image: chessCard,
    title: 'Chess tour',
    type: 'Chess',
    date: '23.02.2026',
    time: '15:00',
    participants: '12/12',
  },
];

const myTournaments = [
  {
    image: chessCard,
    title: 'Chess tour',
    type: 'Chess',
    date: '23.02.2026',
    time: '15:00',
    participants: '12/12',
  },
  {
    image: tennisCard,
    title: 'Tennis tour',
    type: 'Tennis',
    date: '28.06.2026',
    time: '16:00',
    participants: '6/6',
  },
  {
    image: cs2Card,
    title: 'Counter strike 2',
    type: 'Game',
    date: '24.05.2026',
    time: '18:00',
    participants: '32/64',
  },
  {
    image: forzaCard,
    title: 'Forza horizon 5',
    type: 'Game',
    date: '21.03.2026',
    time: '20:00',
    participants: '16/16',
  },
];
</script>

<style scoped>
.role-dashboard {
  background: #252e35;
  color: #fffcf2;
}

.role-dashboard__hero {
  position: relative;
  min-height: 458px;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}

.role-dashboard__overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(
    180deg,
    rgba(37, 46, 53, 0.18) 0%,
    rgba(21, 49, 206, 0.28) 35%,
    rgba(21, 49, 206, 0.42) 100%
  );
}

.role-dashboard__hero-content {
  position: relative;
  z-index: 1;
  padding: 162px 24px 86px;
  text-align: center;
}

.role-dashboard__title {
  margin: 0 0 15px;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.1;
}

.role-dashboard__subtitle {
  margin: 0;
  font-size: 16px;
  line-height: 1.4;
  opacity: 0.92;
}

.role-dashboard__main {
  background: #2d363d;
}

.dashboard-section__inner {
  width: min(1224px, calc(100% - 64px));
  margin: 0 auto;
}

.dashboard-section--active {
  padding-top: 64px;
  padding-bottom: 0;
}

.dashboard-section--my {
  padding-top: 100px;
  padding-bottom: 159px;
}

.dashboard-section__title {
  margin: 0 0 20px;
  text-align: center;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.1;
}

.dashboard-section__subtitle {
  margin: 0 0 42px;
  text-align: center;
  font-size: 16px;
  line-height: 1.4;
  opacity: 0.9;
}

.dashboard-section__grid {
  display: grid;
  grid-template-columns: repeat(4, 230px);
  justify-content: center;
  gap: 32px;
}

.dashboard-section__actions {
  display: flex;
  justify-content: center;
  gap: 28px;
  margin-top: 60px;
}

.dashboard-section__actions--single {
  gap: 0;
}

.dashboard-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  width: 280px;
  height: 46px;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition:
    background 0.2s ease,
    color 0.2s ease,
    border-color 0.2s ease;
}

.dashboard-button--blue {
  border: 1px solid #1531ce;
  background: #1531ce;
  color: #fffcf2;
}

.dashboard-button--blue:hover {
  background: #1531ce;
  color: #fffcf2;
}

.dashboard-button--blue:active,
.dashboard-button--blue:focus-visible {
  background: transparent;
  color: #1531ce;
  border-color: #1531ce;
  outline: none;
}

.dashboard-button--orange {
  border: 1px solid #ff9800;
  background: #ff9800;
  color: #fffcf2;
}

.dashboard-button--orange:hover {
  background: #ff9800;
  color: #fffcf2;
}

.dashboard-button--orange:active,
.dashboard-button--orange:focus-visible {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
  outline: none;
}

.dashboard-button__icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  line-height: 1;
}

@media (max-width: 1200px) {
  .dashboard-section__grid {
    grid-template-columns: repeat(2, 230px);
  }
}

@media (max-width: 960px) {
  .dashboard-section__inner {
    width: min(100%, calc(100% - 32px));
  }

  .role-dashboard__hero-content {
    padding: 132px 20px 72px;
  }

  .role-dashboard__title {
    font-size: 40px;
  }

  .dashboard-section__title {
    font-size: 40px;
  }

  .dashboard-section__actions {
    flex-direction: column;
    align-items: center;
  }

  .dashboard-section--my {
    padding-bottom: 100px;
  }
}

@media (max-width: 640px) {
  .role-dashboard__hero-content {
    padding: 110px 16px 56px;
  }

  .role-dashboard__title {
    font-size: 32px;
  }

  .role-dashboard__subtitle {
    font-size: 15px;
  }

  .dashboard-section--active {
    padding-top: 48px;
  }

  .dashboard-section--my {
    padding-top: 72px;
    padding-bottom: 72px;
  }

  .dashboard-section__title {
    font-size: 32px;
  }

  .dashboard-section__grid {
    grid-template-columns: 1fr;
    justify-items: center;
  }

  .dashboard-button {
    width: 100%;
    max-width: 320px;
  }
}
</style>