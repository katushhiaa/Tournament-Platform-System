<script setup lang="ts">
import { computed, ref } from 'vue'
import type { Participant } from '../../types/Participant'

const props = defineProps<{
  participants: Participant[]
}>()

const PAGE_SIZE = 5
const currentPage = ref(1)

const totalPages = computed(() =>
  Math.max(1, Math.ceil(props.participants.length / PAGE_SIZE))
)

const paginated = computed(() => {
  const start = (currentPage.value - 1) * PAGE_SIZE
  return props.participants.slice(start, start + PAGE_SIZE)
})

const goTo = (page: number) => {
  if (page >= 1 && page <= totalPages.value) currentPage.value = page
}
</script>

<template>
  <section class="participants">
    <h2 class="title">Participants</h2>

    <div class="list">
      <div
        v-for="participant in paginated"
        :key="participant.id"
        class="participant-card"
      >
        {{ participant.name }}
      </div>
    </div>

    <div v-if="totalPages > 1" class="pagination">
      <button
        class="page arrow"
        :disabled="currentPage === 1"
        @click="goTo(currentPage - 1)"
      >‹</button>

      <button
        v-for="page in totalPages"
        :key="page"
        class="page"
        :class="{ active: page === currentPage }"
        @click="goTo(page)"
      >
        {{ page }}
      </button>

      <button
        class="page arrow"
        :disabled="currentPage === totalPages"
        @click="goTo(currentPage + 1)"
      >›</button>
    </div>
  </section>
</template>

<style scoped>
.participants {
  margin-top: 104px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.title {
  font-size: 32px;
  font-weight: 600;
  color: white;
  margin-bottom: 57px;
}

.list {
  display: flex;
  flex-direction: column;
  gap: 25px;
}

.participant-card {
  width: 756px;
  height: 55px;
  border-radius: 12px;
  border: 1px solid #1531ce;
  background: rgba(21, 49, 206, 0.35);
  display: flex;
  align-items: center;
  padding-left: 34px;
  color: white;
  font-size: 16px;
  backdrop-filter: blur(6px);
}

.pagination {
  margin-top: 55px;
  margin-bottom: 74px;
  display: flex;
  gap: 12px;
  align-items: center;
}

.page {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  border: 1px solid #1531ce;
  background: transparent;
  color: #1531ce;
  font-size: 12px;
  cursor: pointer;
  transition: background 0.15s, color 0.15s;
}

.page.active {
  background: #1531ce;
  color: white;
}

.page.arrow {
  font-size: 16px;
  line-height: 1;
}

.page:disabled {
  opacity: 0.3;
  cursor: not-allowed;
}

@media (max-width: 900px) {
  .participant-card {
    width: 90vw;
  }
}
</style>