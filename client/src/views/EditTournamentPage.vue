<template>
  <main class="edit-tournament-page">
    <AppHeader />

    <section class="edit-tournament-page__hero">
      <h1>Edit tournament</h1>
    </section>

    <section class="edit-tournament-page__content">
      <EditTournamentForm />
    </section>

    <SiteFooter />
  </main>
</template>

<script setup lang="ts">
import AppHeader from '../components/AppHeader.vue'
import SiteFooter from '../components/SiteFooter.vue'
import EditTournamentForm from '../components/forms/EditTournamentForm.vue'
</script>

<style scoped>
.edit-tournament-page {
  min-height: 100vh;
  background: #151d22;
  color: #fffcf2;
}

.edit-tournament-page__hero {
  min-height: 312px;
  padding: 118px 16px 0;
  background:
    linear-gradient(rgba(0, 10, 32, 0.35), rgba(0, 10, 32, 0.65)),
    url('../assets/Background_2.png') center / cover no-repeat;
  text-align: center;
}

.edit-tournament-page__hero h1 {
  margin: 0;
  font-size: 48px;
  font-weight: 800;
}

.edit-tournament-page__content {
  margin-top: -30px;
  padding-bottom: 152px;
}
</style>