<template>
  <header class="dashboard-header">
    <div class="dashboard-header__brand">
      <img src="../../assets/logo-icon.png" alt="Zvytyaha logo" class="dashboard-header__logo" />
      <span class="dashboard-header__title">ZVYTIAHA</span>
    </div>

    <nav class="dashboard-header__nav">
      <a href="#top" class="dashboard-header__link dashboard-header__link--active">Home</a>
      <a href="#active-tournaments" class="dashboard-header__link">Browse Tournaments</a>
      <a href="#my-tournaments" class="dashboard-header__link">My Tournaments</a>
    </nav>

    <button class="dashboard-header__leave" @click="handleLeave">Leave</button>
  </header>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { authStore } from '../../state/authStore';

const router = useRouter();

const handleLeave = () => {
  authStore.clearAuth();
  router.push('/');
};
</script>

<style scoped>
.dashboard-header {
  position: absolute;
  top: 22px;
  left: 28px;
  right: 28px;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  min-height: 56px;
  padding: 0 18px;
  border-radius: 12px;
  background: #1531ce;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.25);
}

.dashboard-header__brand {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 180px;
}

.dashboard-header__logo {
  width: 14px;
  height: 18px;
  object-fit: contain;
  flex-shrink: 0;
}

.dashboard-header__title {
  color: #fffcf2;
  font-size: 15px;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.dashboard-header__nav {
  display: flex;
  align-items: center;
  gap: 28px;
  flex: 1;
}

.dashboard-header__link {
  color: #fffcf2;
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
  opacity: 0.95;
}

.dashboard-header__link--active {
  text-decoration: underline;
  text-underline-offset: 4px;
}

.dashboard-header__leave {
  min-width: 88px;
  height: 40px;
  padding: 0 18px;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
  color: #fffcf2;
  border: 1px solid rgba(255, 252, 242, 0.75);
  background: transparent;
}

@media (max-width: 1024px) {
  .dashboard-header {
    flex-wrap: wrap;
    justify-content: center;
    padding: 14px 18px;
    height: auto;
  }

  .dashboard-header__nav {
    justify-content: center;
    flex: unset;
  }
}

@media (max-width: 768px) {
  .dashboard-header {
    position: static;
    margin: 16px;
  }

  .dashboard-header__brand,
  .dashboard-header__nav,
  .dashboard-header__leave {
    width: 100%;
    justify-content: center;
  }
}
</style>