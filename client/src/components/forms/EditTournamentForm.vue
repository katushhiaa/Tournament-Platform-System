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

            <div
              ref="sportDropdownRef"
              class="custom-select"
              :class="{
                'custom-select--open': sportDropdownOpen,
                'custom-select--disabled': isRegistrationClosed,
              }"
            >
              <button
                type="button"
                class="custom-select__trigger"
                :disabled="isRegistrationClosed"
                @click="!isRegistrationClosed && (sportDropdownOpen = !sportDropdownOpen)"
              >
                <span :class="{ 'custom-select__placeholder': !form.sport }">
                  {{ selectedSportName || 'Select sport type' }}
                </span>
                <img
                  :src="dropdownIcon"
                  alt=""
                  class="custom-select__arrow"
                  :class="{ 'custom-select__arrow--open': sportDropdownOpen }"
                />
              </button>

              <ul v-if="sportDropdownOpen && !isRegistrationClosed" class="custom-select__list">
                <li
                  v-for="sport in sports"
                  :key="sport.id"
                  class="custom-select__item"
                  :class="{ 'custom-select__item--selected': form.sport === sport.id }"
                  @click="selectSport(sport)"
                >
                  {{ sport.name }}
                </li>
              </ul>
            </div>
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
      <textarea v-model="form.description" />
    </div>

    <div class="edit-form__card">
      <label>Conditions</label>
      <textarea v-model="form.conditions" />
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

    <AddPlayersModal
      v-if="showAddModal"
      :tournament-id="tournamentId"
      @close="showAddModal = false"
      @player-added="handlePlayerAdded"
    />

    <ConfirmModal
      v-if="showDisqualifyConfirm && participantToDisqualify"
      :message="`Disqualify ${participantToDisqualify.name}? This action cannot be undone.`"
      confirm-text="Disqualify"
      :confirm-danger="true"
      @confirm="confirmDisqualify"
      @cancel="showDisqualifyConfirm = false; participantToDisqualify = null"
    />

    <AppToast v-if="errorToast" :message="errorToast" type="error" />
    <AppToast v-if="toast" :message="toast" type="success" />

    <div class="edit-form__actions">
      <button type="submit" class="edit-form__submit" :disabled="isSubmitting">
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
import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

import { tournamentService } from '../../services/tournamentService'
import { participationService } from '../../services/participationService'
import type { Participant } from '../../types/Participant'
import type { IThemeOption } from '../../types/Tournament'

import uploadBannerIcon from '../../assets/icons/Upload Banner.png'
import nameIcon from '../../assets/icons/Name.png'
import sportIcon from '../../assets/icons/Sport.png'
import dropdownIcon from '../../assets/icons/drop_down_list.png'
import peopleIcon from '../../assets/icons/people.png'
import dateIcon from '../../assets/icons/date.png'
import timeIcon from '../../assets/icons/time.png'

import AddPlayersModal from '../modals/AddPlayersModal.vue'
import ConfirmModal from '../ui/ConfirmModal.vue'
import AppToast from '../ui/AppToast.vue'

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

const isDisqualifying = ref<string | null>(null)
const tournamentStatus = ref('')

const toast = ref('')
const errorToast = ref('')

const showDisqualifyConfirm = ref(false)
const participantToDisqualify = ref<Participant | null>(null)

const sports = ref<IThemeOption[]>([])
const sportDropdownOpen = ref(false)
const sportDropdownRef = ref<HTMLElement | null>(null)

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

const selectedSportName = computed(() =>
  sports.value.find(s => s.id === form.sport)?.name ?? ''
)

const selectSport = (sport: IThemeOption) => {
  form.sport = sport.id
  sportDropdownOpen.value = false
}

const handleOutsideClick = (e: MouseEvent) => {
  if (sportDropdownRef.value && !sportDropdownRef.value.contains(e.target as Node)) {
    sportDropdownOpen.value = false
  }
}

const toDatetimeLocal = (iso: string): string => {
  if (!iso) return ''
  const d = iso.endsWith('Z') || iso.includes('+') 
    ? new Date(iso) 
    : new Date(iso + 'Z')
  const offset = d.getTimezoneOffset()
  const local = new Date(d.getTime() - offset * 60000)
  return local.toISOString().slice(0, 16)
}

const showError = (msg: string) => {
  errorToast.value = msg
  setTimeout(() => { errorToast.value = '' }, 4000)
}

const handleFileChange = async (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  previewImage.value = URL.createObjectURL(file)
  try {
    isUploading.value = true
    const formData = new FormData()
    formData.append('file', file)
    await tournamentService.uploadTournamentImage(props.tournamentId, formData)
  } catch {
    showError('Failed to upload image. Please try again.')
    previewImage.value = null
  } finally {
    isUploading.value = false
  }
}

const disqualifyParticipant = (participant: Participant) => {
  participantToDisqualify.value = participant
  showDisqualifyConfirm.value = true
}

const confirmDisqualify = async () => {
  const participant = participantToDisqualify.value
  if (!participant) return
  showDisqualifyConfirm.value = false
  isDisqualifying.value = participant.id
  try {
    await participationService.removeParticipant(props.tournamentId, participant.userId)
    participants.value = participants.value.filter(p => p.id !== participant.id)
  } catch (e: any) {
    showError(e?.message ?? 'Failed to disqualify participant.')
  } finally {
    isDisqualifying.value = null
    participantToDisqualify.value = null
  }
}

const handlePlayerAdded = async () => {
  participants.value = await participationService.getTournamentParticipants(props.tournamentId)
  showAddModal.value = false
}

const handleSubmit = async () => {
  if (Number(form.maxParticipants) < 1) {
    showError('Participants count must be at least 1')
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
      showError(e.message ?? 'Editing is blocked. Tournament is already active.')
    } else {
      showError('Failed to save changes. Please try again.')
    }
  } finally {
    isSubmitting.value = false
  }
}

onMounted(async () => {
  document.addEventListener('click', handleOutsideClick)
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
    const closedStatuses = ['registration_closed', 'in_progress', 'completed']
    isRegistrationClosed.value = closedStatuses.includes(tournament.status)
    tournamentStatus.value = tournament.status
  } catch (e) {
    console.error('Failed to load tournament for edit', e)
  } finally {
    isLoading.value = false
  }
})

onUnmounted(() => {
  document.removeEventListener('click', handleOutsideClick)
})
</script>

<style scoped>
.edit-form {
  width: 1109px;
  max-width: calc(100% - 32px);
  margin: 0 auto;
  color: #fffcf2;
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
  align-content: center;
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

.edit-form__input-wrapper input {
  width: 334px;
  height: 44px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: rgba(21, 49, 206, 0.47);
  color: white;
  padding: 0 44px 0 48px;
  font-size: 13px;
  outline: none;
  box-sizing: border-box;
}

.edit-form__icon {
  position: absolute;
  top: 50%;
  left: 14px;
  width: 24px;
  transform: translateY(-50%);
  filter: brightness(0) invert(1);
  z-index: 2;
  pointer-events: none;
}

.custom-select {
  position: relative;
  width: 334px;
}

.custom-select__trigger {
  width: 100%;
  height: 44px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
  padding: 0 44px 0 48px;
  font-size: 13px;
  text-align: left;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-sizing: border-box;
  transition: border-color 0.2s;
}

.custom-select--open .custom-select__trigger {
  border-color: #ff9800;
  border-bottom-left-radius: 0;
  border-bottom-right-radius: 0;
}

.custom-select--disabled .custom-select__trigger {
  opacity: 0.6;
  cursor: not-allowed;
}

.custom-select__placeholder {
  color: rgba(255, 252, 242, 0.65);
}

.custom-select__arrow {
  width: 20px;
  height: 20px;
  object-fit: contain;
  flex-shrink: 0;
  filter: brightness(0) invert(1);
  transition: transform 0.2s;
  pointer-events: none;
}

.custom-select__arrow--open {
  transform: rotate(180deg);
}

.custom-select__list {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  z-index: 200;
  list-style: none;
  margin: 0;
  padding: 4px 0;
  border: 1px solid #1531ce;
  border-top: none;
  border-bottom-left-radius: 10px;
  border-bottom-right-radius: 10px;
  background: #1a2540;
  max-height: 220px;
  overflow-y: auto;
  scrollbar-width: thin;
  scrollbar-color: #1531ce transparent;
}

.custom-select__list::-webkit-scrollbar {
  width: 4px;
}

.custom-select__list::-webkit-scrollbar-thumb {
  background: #1531ce;
  border-radius: 4px;
}

.custom-select__item {
  padding: 10px 20px;
  font-size: 13px;
  color: #fffcf2;
  cursor: pointer;
  transition: background 0.15s;
}

.custom-select__item:hover {
  background: rgba(21, 49, 206, 0.5);
}

.custom-select__item--selected {
  background: rgba(255, 152, 0, 0.2);
  color: #ff9800;
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
  font-size: 13px;
  outline: none;
  resize: vertical;
  box-sizing: border-box;
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
  font-size: 14px;
  font-weight: 600;
  transition: background 0.2s, color 0.2s;
}

.edit-form__add-player:hover {
  background: rgba(21, 49, 206, 0.15);
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
  color: #fffcf2;
}

.edit-form__disqualify {
  border: none;
  background: transparent;
  color: white;
  text-decoration: underline;
  cursor: pointer;
  font-size: 14px;
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
  border: 1px solid #ff9800;
  border-radius: 10px;
  background: #ff9800;
  color: white;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
}

.edit-form__submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.edit-form__cancel {
  width: 130px;
  height: 46px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: transparent;
  color: #1531ce;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
}

@media (max-width: 1100px) {
  .edit-form {
    width: calc(100% - 32px);
  }

  .edit-form__top-card {
    grid-template-columns: 1fr;
  }

  .edit-form__upload {
    width: 100%;
  }

  .edit-form__grid {
    grid-template-columns: 1fr;
  }

  .edit-form__input-wrapper input,
  .custom-select {
    width: 100%;
  }

  .edit-form__participants,
  .edit-form__participant {
    width: 100%;
  }
}
</style>