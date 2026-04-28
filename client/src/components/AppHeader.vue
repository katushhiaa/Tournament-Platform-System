<template>
  <header class="header">
    <div class="header__brand">
      <img src="../assets/logo-icon.png" alt="Zvytyaha logo" class="header__logo" />
      <span class="header__title">ZVYTIAHA</span>
    </div>

    <nav class="header__nav">
      <router-link
        :to="homeRoute"
        class="nav-link"
        exact-active-class="active"
      >
        Home
      </router-link>

      <router-link to="/tournaments" class="nav-link">
        Browse Tournaments
      </router-link>
    </nav>

    <div class="header__actions">
      <template v-if="isAuthenticated">
        <button class="header__btn header__btn--leave" @click="handleLeave">
          Leave
        </button>
      </template>

      <template v-else>
        <router-link
          to="/login"
          class="header__btn header__btn--login header__btn-link"
        >
          Log In
        </router-link>

        <router-link
          to="/register"
          class="header__btn header__btn--signup header__btn-link"
        >
          Sign Up
        </router-link>
      </template>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { authStore } from '../state/authStore';

const router = useRouter();

const isAuthenticated = computed(() => authStore.state.isAuthenticated);

const homeRoute = computed(() => {
  const rawRole = authStore.state.user?.role ?? '';
  const role = String(rawRole).toLowerCase();

  if (role === 'organizer') {
    return { name: 'organizer-dashboard' };
  }

  if (role === 'player') {
    return { name: 'player-dashboard' };
  }

  return { name: 'home' };
});

const handleLeave = () => {
  authStore.clearAuth();
  router.push('/');
};
</script>

<style scoped>
.header {
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

.header__brand {
  display: flex;
  align-items: center;
  gap: 8px;
  min-width: 180px;
}

.header__logo {
  width: 30px;
  height: 30px;
  object-fit: contain;
  flex-shrink: 0;
}

.header__title {
  color: #fffcf2;
  font-size: 15px;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.header__nav {
  display: flex;
  align-items: center;
  gap: 28px;
  flex: 1;
}

.nav-link {
  color: #fffcf2;
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
  opacity: 0.95;
}

.nav-link.active {
  text-decoration: underline;
  text-underline-offset: 4px;
}

.header__actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header__btn {
  min-width: 96px;
  height: 40px;
  padding: 0 24px;
  border-radius: 18px;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition:
    background 0.2s ease,
    color 0.2s ease,
    border-color 0.2s ease;
}

.header__btn-link {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
}

/* LOG IN — filled by default, outline when pressed */
.header__btn--login {
  border: 1px solid #ff9800;
  background: #ff9800;
  color: #fffcf2;
}

.header__btn--login:hover {
  background: #ff9800;
  color: #fffcf2;
}

.header__btn--login:active,
.header__btn--login:focus-visible {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
  outline: none;
}

/* SIGN UP — outline by default, filled when pressed */
.header__btn--signup {
  border: 1px solid #ff9800;
  background: transparent;
  color: #ff9800;
}

.header__btn--signup:hover {
  background: rgba(255, 152, 0, 0.1);
  color: #ff9800;
}

.header__btn--signup:active,
.header__btn--signup:focus-visible {
  background: #ff9800;
  color: #fffcf2;
  border-color: #ff9800;
  outline: none;
}

/* LEAVE — white outline like in dashboard mock */
.header__btn--leave {
  border: 1px solid rgba(255, 252, 242, 0.75);
  background: transparent;
  color: #fffcf2;
}

.header__btn--leave:hover {
  background: rgba(255, 252, 242, 0.08);
  color: #fffcf2;
}

.header__btn--leave:active,
.header__btn--leave:focus-visible {
  background: #fffcf2;
  color: #1531ce;
  border-color: #fffcf2;
  outline: none;
}

@media (max-width: 1024px) {
  .header {
    flex-wrap: wrap;
    justify-content: center;
    padding: 14px 18px;
    height: auto;
  }

  .header__nav {
    justify-content: center;
    flex: unset;
  }
}

@media (max-width: 768px) {
  .header {
    position: static;
    margin: 16px;
  }

  .header__brand,
  .header__nav,
  .header__actions {
    width: 100%;
    justify-content: center;
  }
}
</style>