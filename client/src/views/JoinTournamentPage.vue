<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import { participationService } from '../services/participationService'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const isProcessing = ref(false) 

onMounted(async () => {
  if (isProcessing.value) return 
  isProcessing.value = true

  const id = route.params.id as string

  if (!authStore.isAuthenticated) {
    router.push({ path: '/login', query: { redirect: `/join/${id}` } })
    return
  }

  try {
    await participationService.addParticipant(id, authStore.currentUser!.userId)
    sessionStorage.setItem('joinToast', `You have successfully joined the tournament!`)
 } catch (e: any) {
    const msg = e?.response?.data?.message ?? 'Failed to join the tournament.'
    sessionStorage.setItem('joinToastError', msg)
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
