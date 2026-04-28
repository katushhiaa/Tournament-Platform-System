<template>
  <article class="tournament-card">
    <div class="tournament-card__image-wrapper">
      <img :src="image" :alt="title" class="tournament-card__image" />
      <span class="tournament-card__badge">Active</span>
    </div>

    <div class="tournament-card__content">
      <h3 class="tournament-card__title">{{ title }}</h3>
      <p class="tournament-card__text">Sport Type: {{ type }}</p>
      <p class="tournament-card__text">Date: {{ date }}</p>
      <p class="tournament-card__text">Time: {{ time }}</p>
      <p class="tournament-card__text">Participants: {{ participants }}</p>

      <div class="tournament-card__actions">
        <button class="tournament-card__button">View</button>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
defineProps<{
  image: string;
  title: string;
  type: string;
  date: string;
  time: string;
  participants: string;
}>();
</script>

<style scoped>
.tournament-card {
  width: 230px;
  min-height: 306px;
  overflow: hidden;
  border-radius: 18px;
  background: #252e35;
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.2);
}

.tournament-card__image-wrapper {
  position: relative;
  width: 230px;
  height: 157px;
}

.tournament-card__image {
  display: block;
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.tournament-card__badge {
  position: absolute;
  top: 10px;
  right: 10px;
  border-radius: 999px;
  padding: 4px 10px;
  background: #84c082;
  color: #fffcf2;
  font-size: 10px;
  font-weight: 600;
  line-height: 1;
}

.tournament-card__content {
  padding: 14px 10px 12px;
}

.tournament-card__title {
  margin: 0 0 9px;
  color: #fffcf2;
  font-size: 13px;
  font-weight: 700;
  line-height: 1.3;
}

.tournament-card__text {
  margin: 0 0 9px;
  color: #fffcf2;
  font-size: 10px;
  line-height: 1.4;
  opacity: 0.9;
}

.tournament-card__actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 6px;
}

.tournament-card__button {
  width: 70px;
  height: 25px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: transparent;
  color: #fffcf2;
  font-size: 12px;
  font-weight: 700;
  line-height: 1;
  cursor: pointer;
}
</style>