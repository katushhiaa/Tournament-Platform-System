<script setup lang="ts">
import type { ITournament } from '../../types/Tournament'

defineProps<{
  tournament: ITournament
}>()
</script>

<template>
  <section class="overview">
    <h2 class="title">Overview</h2>

    <div class="text">
      {{ tournament.description ?? 'No description provided.' }}
    </div>

    <template v-if="tournament.conditions">
      <h2 class="title conditions-title">Conditions</h2>
      <div class="text">
        {{ tournament.conditions }}
      </div>
    </template>
  </section>
</template>

<style scoped>
.overview {
  padding-top: 104px;
  padding-bottom: 120px;
}

.title {
  text-align: center;
  font-size: 32px;
  font-weight: 600;
  margin-bottom: 37px;
}

.conditions-title {
  margin-top: 64px;
}

.text {
  white-space: pre-line;
  font-size: 16px;
  line-height: 1.8;
  margin-left: 156px;
  margin-right: 152px;
  color: #fffcf2;
}

@media (max-width: 768px) {
  .text {
    margin-left: 20px;
    margin-right: 20px;
  }
}
</style>