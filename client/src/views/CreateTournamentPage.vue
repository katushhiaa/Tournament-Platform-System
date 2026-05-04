<template>
  <main class="create-tournament-page">
    <AppHeader />

    <section class="create-tournament-page__hero">
      <h1>Create tournament</h1>
    </section>

    <section class="create-tournament-page__content">
      <CreateTournamentForm />
    </section>

    <SiteFooter />
  </main>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue';
import SiteFooter from '../components/SiteFooter.vue';
import CreateTournamentForm from '../components/forms/CreateTournamentForm.vue';
</script>

<style scoped>
.create-tournament-page {
  min-height: 100vh;
  background: #151d22;
  color: #fffcf2;
}

.create-tournament-page__hero {
  min-height: 270px;
  padding: 126px 16px 0;
  background:
    linear-gradient(rgba(0, 10, 32, 0.2), rgba(0, 10, 32, 0.75)),
    url('../assets/hero-bg.png') center / cover no-repeat;
  text-align: center;
}

.create-tournament-page__hero h1 {
  margin: 0;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.15;
}

.create-tournament-page__content {
  margin-top: 30px;
  padding-bottom: 152px;
}

@media (max-width: 900px) {
  .create-tournament-page__hero {
    padding-top: 110px;
  }

  .create-tournament-page__hero h1 {
    font-size: 36px;
  }
}

.create-tournament-page__hero {
  min-height: 312px;
  padding: 118px 16px 0;
  background:
    linear-gradient(rgba(0, 10, 32, 0.35), rgba(0, 10, 32, 0.65)),
    url('../assets/Background_2.png') center / cover no-repeat;
  text-align: center;
}

.create-tournament-page__hero h1 {
  margin: 0;
  font-size: 48px;
  font-weight: 800;
  line-height: 1.15;
}

.create-tournament-page__content {
  margin-top: -30px;
  padding-bottom: 152px;
}
</style>