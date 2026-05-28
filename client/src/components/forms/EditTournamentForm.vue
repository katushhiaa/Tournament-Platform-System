<template>
  <form class="edit-form" @submit.prevent="handleSubmit">

    <div class="edit-form__top-card">

      <button
        type="button"
        class="edit-form__upload"
        :class="{ 'edit-form__upload--has-image': previewImage }"
        @click="fileInput?.click()"
      >
        <template v-if="isUploading">
          <p class="edit-form__upload-title">Uploading...</p>
        </template>

        <template v-else-if="previewImage">
          <img :src="previewImage" class="edit-form__upload-preview" />
        </template>

        <template v-else>
          <img :src="uploadBannerIcon" class="edit-form__upload-icon" />
          <p class="edit-form__upload-title">Upload Banner</p>
          <p class="edit-form__upload-text">PNG, JPG up to 5MB</p>
          <p class="edit-form__upload-text">Recommended 16:9</p>
        </template>
      </button>

      <input
        ref="fileInput"
        type="file"
        accept="image/png,image/jpeg"
        style="display:none"
        @change="handleFileChange"
      />

      <div class="edit-form__grid">

        <div class="edit-form__field">
          <label>Tournament Name</label>

          <div class="edit-form__input-wrapper">
            <img :src="nameIcon" class="edit-form__icon" />

            <input
              v-model="form.title"
              type="text"
              :readonly="isRegistrationClosed"
              placeholder="Enter tournament name"
            />
          </div>
        </div>

        <div class="edit-form__field">
          <label>Date start</label>

          <div class="edit-form__input-wrapper">
            <img :src="dateIcon" class="edit-form__icon" />

            <input
              v-model="form.startDate"
              type="datetime-local"
              :readonly="isRegistrationClosed"
            />
          </div>
        </div>

        <div class="edit-form__field">
          <label>Sport Type</label>

          <div class="edit-form__input-wrapper">
            <img :src="sportIcon" class="edit-form__icon" />

            <select v-model="form.sport" :disabled="isRegistrationClosed">
                  <option
                    v-for="sport in sports"
                    :key="sport.id"
                    :value="sport.id"
                  >
                    {{ sport.name }}
                  </option>
                </select>
          </div>
        </div>

        <div class="edit-form__field">
          <label>Date end</label>

          <div class="edit-form__input-wrapper">
            <img :src="dateIcon" class="edit-form__icon" />

            <input
              v-model="form.endDate"
              type="datetime-local"
              :readonly="isRegistrationClosed"
            />
          </div>
        </div>

        <div class="edit-form__field">
          <label>Participants max count</label>

          <div class="edit-form__input-wrapper">
            <img :src="peopleIcon" class="edit-form__icon" />

            <input
              v-model="form.maxParticipants"
              type="number"
              :readonly="isRegistrationClosed"
            />
          </div>
        </div>

        <div class="edit-form__field">
          <label>End of Registration</label>

          <div class="edit-form__input-wrapper">
            <img :src="timeIcon" class="edit-form__icon" />

            <input
              v-model="form.registrationCloseDate"
              type="datetime-local"
              :readonly="isRegistrationClosed"
            />
          </div>
        </div>

      </div>
    </div>

    <div class="edit-form__card">
      <label>Description</label>

      <textarea
        v-model="form.description"
      />
    </div>

    <div class="edit-form__card">
      <label>Conditions</label>

      <textarea
        v-model="form.conditions"
      />
    </div>

    <div class="edit-form__participants">
      <div class="edit-form__participants-header">
        <h2>Participants</h2>
        <button
          type="button"
          class="edit-form__add-player"
          @click="showAddModal = true"
        >
          Add player
        </button>
      </div>

      <p v-if="isLoading" style="color:rgba(255,255,255,0.6)">Loading...</p>

      <p v-else-if="!participants.length" style="color:rgba(255,255,255,0.6)">
        No participants yet.
      </p>

      <div
        v-else
        v-for="participant in participants"
        :key="participant.id"
        class="edit-form__participant"
      >
        <span>{{ participant.name }}</span>
        <button
          type="button"
          class="edit-form__disqualify"
          :disabled="tournamentStatus !== 'registration_open' || isDisqualifying === participant.id"
          @click="disqualifyParticipant(participant)"
        >
          {{ isDisqualifying === participant.id ? 'Disqualifying...' : 'Disqualify' }}
        </button>
      </div>
    </div>

    <!-- AddPlayerModal -->
   <AddPlayersModal
      v-if="showAddModal"
      :tournament-id="tournamentId"
      @close="showAddModal = false"
      @player-added="handlePlayerAdded"
    />
    <div v-if="toast" class="edit-form__toast">
      {{ toast }}
    </div>

    <div class="edit-form__actions">

      <button
        type="submit"
        class="edit-form__submit"
      >
        {{ isSubmitting ? 'Saving...' : 'Save changes' }}
      </button>

      <button
        type="button"
        class="edit-form__cancel"
        @click="router.push('/my-tournaments')"
      >
        Cancel
      </button>

    </div>

  </form>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

import { tournamentService } from '../../services/tournamentService'
import { participationService } from '../../services/participationService'
import type { Participant } from '../../types/Participant'

import type { IThemeOption } from '../../types/Tournament'
import uploadBannerIcon from '../../assets/icons/Upload Banner.png'
import nameIcon from '../../assets/icons/Name.png'
import sportIcon from '../../assets/icons/Sport.png'
import peopleIcon from '../../assets/icons/people.png'
import dateIcon from '../../assets/icons/date.png'
import timeIcon from '../../assets/icons/time.png'
import AddPlayersModal from '../modals/AddPlayersModal.vue'

const props = defineProps<{
  tournamentId: string
}>()

const router = useRouter()
const isSubmitting = ref(false)
const isLoading = ref(true)
const isRegistrationClosed = ref(false)
const participants = ref<Participant[]>([])
const showAddModal = ref(false)

const fileInput = ref<HTMLInputElement | null>(null)
const previewImage = ref<string | null>(null)
const isUploading = ref(false)

const isDisqualifying = ref<string | null>(null) // зберігає id учасника що дискваліфікується
const tournamentStatus = ref('')

const toast = ref('')

const handleFileChange = async (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return

  // Показати превью
  previewImage.value = URL.createObjectURL(file)

  // Завантажити на сервер
  try {
    isUploading.value = true
    const formData = new FormData()
    formData.append('file', file)
    await tournamentService.uploadTournamentImage(props.tournamentId, formData)
  } catch (e) {
    alert('Failed to upload image. Please try again.')
    previewImage.value = null
  } finally {
    isUploading.value = false
  }
}

const sports = ref<IThemeOption[]>([])
const form = reactive({
  title: '',
  sport: '',
  startDate: '',
  endDate: '',
  registrationCloseDate: '',
  maxParticipants: 0,
  description: '',
  conditions: '',
})

// Форматуємо ISO в datetime-local формат
const toDatetimeLocal = (iso: string): string => {
  if (!iso) return ''
  return iso.slice(0, 16)
}

onMounted(async () => {
  const sportsData = await tournamentService.getSports()
  sports.value = sportsData
  try {
    const [tournament, participantsData] = await Promise.all([
      tournamentService.getTournamentById(props.tournamentId),
      participationService.getTournamentParticipants(props.tournamentId),
    ])

    form.title = tournament.title
    form.sport = tournament.sportId
    form.startDate = toDatetimeLocal(tournament.startDate)
    form.endDate = toDatetimeLocal(tournament.endDate)
    form.registrationCloseDate = toDatetimeLocal(tournament.registrationCloseDate)
    form.maxParticipants = tournament.maxParticipants
    form.description = tournament.description ?? ''
    form.conditions = tournament.conditions ?? ''

    if (tournament.backgroundImg) {
      previewImage.value = tournament.backgroundImg
    }

    participants.value = participantsData

    // Блокуємо редагування якщо реєстрація закрита
    const closedStatuses = ['registration_closed', 'in_progress', 'completed']
    isRegistrationClosed.value = closedStatuses.includes(tournament.status)
    tournamentStatus.value = tournament.status
  } catch (e) {
    console.error('Failed to load tournament for edit', e)
  } finally {
    isLoading.value = false
  }
})

const disqualifyParticipant = async (participant: Participant) => {
  const confirmed = window.confirm(`Disqualify ${participant.name}?`)
  if (!confirmed) return

  isDisqualifying.value = participant.id
  try {
    await participationService.removeParticipant(
      props.tournamentId,
      participant.userId
    )
    participants.value = participants.value.filter(p => p.id !== participant.id)
  } catch (e: any) {
    const msg = e?.message ?? 'Failed to disqualify participant.'
    alert(msg)
  } finally {
    isDisqualifying.value = null
  }
}

const handlePlayerAdded = async () => {
  participants.value = await participationService.getTournamentParticipants(props.tournamentId)
  showAddModal.value = false
}

const handleSubmit = async () => {
  if (Number(form.maxParticipants) < 1) {
    alert('Participants count must be at least 1')
    return
  }

  isSubmitting.value = true
  try {
    await tournamentService.updateTournament(props.tournamentId, {
      title: form.title,
      sport: form.sport,
      startDate: new Date(form.startDate).toISOString(),
      endDate: new Date(form.endDate).toISOString(),
      registrationCloseDate: new Date(form.registrationCloseDate).toISOString(),
      maxParticipants: Number(form.maxParticipants),
      description: form.description || null,
      conditions: form.conditions || null,
    })
    toast.value = 'Changes saved successfully!'
    setTimeout(() => {
      toast.value = ''
      router.push(`/tournaments/${props.tournamentId}`)
    }, 2000)
  } catch (e: any) {
    if (e?.errorCode === 'CONFLICT') {
      alert(e.message ?? 'Editing is blocked. Tournament is already active.')
    } else {
      alert('Failed to save changes. Please try again.')
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.edit-form {
  width: 1109px;
  max-width: calc(100% - 32px);
  margin: 0 auto;
}

.edit-form__top-card {
  display: grid;
  grid-template-columns: 268px 1fr;
  gap: 34px;

  padding: 37px 60px;

  border-radius: 18px;

  background: rgba(37, 46, 53, 0.95);
}

.edit-form__upload {
  width: 268px;
  height: 320px;

  border: 1px dashed #1531ce;
  border-radius: 14px;

  background: rgba(21, 49, 206, 0.18);

  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;

  color: white;

  cursor: pointer;
}

.edit-form__upload-icon {
  width: 64px;
  margin-bottom: 24px;
}

.edit-form__upload-title {
  font-size: 18px;
  font-weight: 700;
}

.edit-form__upload-text {
  font-size: 14px;
}

.edit-form__grid {
  display: grid;
  grid-template-columns: repeat(2, 334px);
  gap: 22px 30px;
}

.edit-form__toast {
  margin: 20px 0 0;
  padding: 14px 24px;
  border-radius: 12px;
  background: #84c082;
  color: #151d22;
  font-size: 16px;
  font-weight: 600;
  text-align: center;
}

.edit-form__field label {
  display: block;
  margin-bottom: 10px;

  font-size: 20px;
  font-weight: 600;
}

.edit-form__input-wrapper {
  position: relative;
}

.edit-form__upload--has-image {
  padding: 0;
  overflow: hidden;
}

.edit-form__upload-preview {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 14px;
}

.edit-form__input-wrapper input,
.edit-form__input-wrapper select {
  width: 334px;
  height: 44px;

  border: 1px solid #1531ce;
  border-radius: 10px;

  background: rgba(21, 49, 206, 0.47);

  color: white;

  padding: 0 44px 0 48px;
}

.edit-form__icon {
  position: absolute;

  top: 50%;
  left: 14px;

  width: 24px;

  transform: translateY(-50%);
}

.edit-form__card {
  margin-top: 28px;

  padding: 34px 64px;

  border-radius: 18px;

  background: rgba(37, 46, 53, 0.95);
}

.edit-form__card label {
  display: block;

  margin-bottom: 12px;

  font-size: 24px;
  font-weight: 700;
}

.edit-form__card textarea {
  width: 100%;
  min-height: 190px;

  border: 1px solid #1531ce;
  border-radius: 14px;

  background: rgba(21, 49, 206, 0.47);

  color: white;

  padding: 18px;
}

.edit-form__participants {
  width: 756px;

  margin: 40px auto 0;
}

.edit-form__participants-header {
  display: flex;
  justify-content: space-between;
  align-items: center;

  margin-bottom: 28px;
}

.edit-form__participants-header h2 {
  font-size: 32px;
}

.edit-form__add-player {
  width: 120px;
  height: 46px;

  border: 1px solid #1531ce;
  border-radius: 10px;

  background: transparent;

  color: #1531ce;

  cursor: pointer;
}

.edit-form__participant {
  width: 756px;
  height: 55px;

  margin-bottom: 25px;
  padding: 0 28px;

  border: 1px solid #1531ce;
  border-radius: 10px;

  background: rgba(21, 49, 206, 0.47);

  display: flex;
  align-items: center;
  justify-content: space-between;
}

.edit-form__disqualify {
  border: none;
  background: transparent;

  color: white;

  text-decoration: underline;

  cursor: pointer;
}

.edit-form__disqualify:hover {
  color: #e53935;
}

.edit-form__disqualify:disabled {
  opacity: 0.35;
  cursor: not-allowed;
  text-decoration: none;
}

.edit-form__actions {
  display: flex;
  justify-content: flex-end;
  gap: 22px;

  margin-top: 40px;
}

.edit-form__submit {
  width: 281px;
  height: 46px;

  border: none;
  border-radius: 10px;

  background: #ff9800;

  color: white;

  cursor: pointer;
}

.edit-form__cancel {
  width: 130px;
  height: 46px;

  border: 1px solid #1531ce;
  border-radius: 10px;

  background: transparent;

  color: #1531ce;

  cursor: pointer;
}
</style>