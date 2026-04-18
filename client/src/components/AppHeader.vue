<template>
  <header class="header">
    <div class="header__brand">
      <img src="../assets/logo-icon.png" alt="Zvytyaha logo" class="header__logo" />
      <span class="header__title">ZVYTYAHA</span>
    </div>

    <nav class="header__nav">
      <a href="#" class="header__link header__link--active">Home</a>
      <a href="#" class="header__link">Browse Tournaments</a>
    </nav>

    <div class="header__actions">
      <button class="header__btn header__btn--ghost">Log In</button>
      <button class="header__btn header__btn--primary">Sign Up</button>
    </div>
  </header>
</template>

<script setup lang="ts"></script>

<style scoped>
.header__logo {
  width: 30px;
  height: 30px;
  object-fit: contain;
  flex-shrink: 0;
}

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
  color: #ff9800;
  font-size: 14px;
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

.header__link {
  color: #fffcf2;
  font-size: 13px;
  font-weight: 600;
  text-decoration: none;
  opacity: 0.95;
}

.header__link--active {
  text-decoration: underline;
  text-underline-offset: 4px;
}

.header__actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.header__btn {
  min-width: 88px;
  height: 40px;
  padding: 0 18px;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 700;
  cursor: pointer;
}

.header__btn--ghost {
  color: #fffcf2;
  border: 1px solid rgba(255, 252, 242, 0.75);
  background: transparent;
}

.header__btn--primary {
  color: #fffcf2;
  border: none;
  background: #ff9800;
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