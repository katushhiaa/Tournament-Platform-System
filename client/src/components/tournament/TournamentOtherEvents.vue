<script setup lang="ts">
import dateIcon from '../../assets/icons/date.png';
const events = [
  {
    date: '30.05.2026',
    title: 'Registration opens',
    text: 'Teams can start registering for the tournament.',
  },
  {
    date: '12.05.2026',
    title: 'Check-in deadline',
    text: 'All teams must confirm their participation.',
  },
  {
    date: '15.05.2026',
    title: 'Group stage starts',
    text: 'The first matches of the group stage begin.',
  },
  {
    date: '24.05.2026',
    title: 'Grand finals',
    text: 'The final showdown. Who will be the champion?',
  },
]
</script>

<template>
  <section class="events">
    <h2 class="title">
      Other events
    </h2>

    <div class="list">
      <div
        v-for="event in events"
        :key="event.title"
        class="card"
      >
        <div class="icon">
          <img :src="dateIcon" alt="Date" />
        </div>

        <div class="date">
          {{ event.date }}
        </div>

        <div class="content">
          <h3>
            {{ event.title }}
          </h3>

          <p>
            {{ event.text }}
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.events {
  padding: 80px 80px 140px;
}

.title {
  text-align: center;
  font-size: 32px;
  font-weight: 600;
  color: white;

  margin-bottom: 72px;
}

.list {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 30px;
}

.card {
  width: 756px;
  min-height: 95px;

  background: rgba(21, 49, 206, 0.35);

  border: 1px solid rgba(77, 110, 255, 0.5);

  border-radius: 12px;

  padding: 24px 28px;

  display: flex;
  align-items: flex-start;
}

.icon {
  font-size: 40px;
  margin-right: 40px;
  margin-top: 15px;
}

.date {
  width: 120px;
  margin-top: 35px;
  font-size: 16px;
  color: white;
}

.content h3 {
  font-size: 16px;
  margin-bottom: 10px;
  color: white;
}

.content p {
  font-size: 14px;
  line-height: 1.5;
  color: rgba(255,255,255,0.85);
}

@media (max-width: 900px) {
  .card {
    width: 100%;
  }

  .events {
    padding: 60px 16px 120px;
  }
}
</style>