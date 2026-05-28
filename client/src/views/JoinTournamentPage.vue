<script setup lang="ts">
import { onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import { participationService } from '../services/participationService'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

onMounted(async () => {
  const id = route.params.id as string

  // Гість — перенаправляємо на логін, після логіну повернемось сюди
  if (!authStore.isAuthenticated) {
    router.push({ path: '/login', query: { redirect: `/join/${id}` } })
    return
  }

  try {
    await participationService.addParticipant(id, authStore.currentUser!.userId)
    sessionStorage.setItem('joinToast', `You have successfully joined the tournament!`)
  } catch (e: any) {
    // 409 — вже зареєстрований, просто йдемо на деталі
    console.warn('Join error (may already be registered):', e)
  }

  router.push(`/tournaments/${id}`)
})
</script>

<template>
  <div class="join-page">
    <p class="join-page__text">Joining tournament...</p>
  </div>
</template>

<style scoped>
.join-page {
  min-height: 100vh;
  background: #151d22;
  color: #fffcf2;
  display: flex;
  align-items: center;
  justify-content: center;
}

.join-page__text {
  font-size: 24px;
  opacity: 0.7;
}
</style>