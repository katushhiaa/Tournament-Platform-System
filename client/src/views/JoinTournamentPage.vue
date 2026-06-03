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

  if (!authStore.isAuthenticated) {
    router.push({ path: '/login', query: { redirect: `/join/${id}` } })
    return
  }

  try {
    await participationService.addParticipant(id, authStore.currentUser!.userId)
    sessionStorage.setItem('joinToast', 'You have successfully joined the tournament!')
    router.push(`/tournaments/${id}`)
  } catch (e: any) {
    const status = e?.response?.status
    let message = 'Something went wrong. Please try again.'
    if (status === 409) {
      const msg = e?.response?.data?.error?.message ?? ''
      if (msg.toLowerCase().includes('no spot') || msg.toLowerCase().includes('maximum')) {
        message = 'No spots available. The tournament is full.'
      } else {
        message = 'Registration for this tournament is closed.'
      }
    } else if (status === 403) {
      message = 'You do not have permission to join this tournament.'
    } else if (!status) {
      message = 'Connection error. Please check your internet connection.'
    }
    sessionStorage.setItem('joinToastError', message)
    router.push(`/tournaments/${id}`)
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