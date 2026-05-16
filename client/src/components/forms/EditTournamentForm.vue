<template>
  <form class="edit-form" @submit.prevent="handleSubmit">

    <div class="edit-form__top-card">

      <button
        type="button"
        class="edit-form__upload"
      >
        <img :src="uploadBannerIcon" class="edit-form__upload-icon" />

        <p class="edit-form__upload-title">
          Upload Banner
        </p>

        <p class="edit-form__upload-text">
          PNG, JPG up to 5MB
        </p>

        <p class="edit-form__upload-text">
          Recommended 16:9
        </p>
      </button>

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

            <select
              v-model="form.sport"
              :disabled="isRegistrationClosed"
            >
              <option value="1">Game</option>
              <option value="2">Chess</option>
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
        >
          Add player
        </button>
      </div>

      <div
        v-for="player in participants"
        :key="player"
        class="edit-form__participant"
      >
        <span>{{ player }}</span>

        <button
          type="button"
          class="edit-form__disqualify"
          @click="removeParticipant(player)"
        >
          Disqualify
        </button>
      </div>

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
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

import uploadBannerIcon from '../../assets/icons/Upload Banner.png'
import nameIcon from '../../assets/icons/Name.png'
import sportIcon from '../../assets/icons/Sport.png'
import peopleIcon from '../../assets/icons/people.png'
import dateIcon from '../../assets/icons/date.png'
import timeIcon from '../../assets/icons/time.png'

const router = useRouter()

const isSubmitting = ref(false)

const isRegistrationClosed = ref(false)

const form = reactive({
  title: 'Counter strike 2',
  sport: '1',
  startDate: '2026-05-24T12:00',
  endDate: '2026-06-23T18:00',
  registrationCloseDate: '2026-05-20T23:59',
  maxParticipants: 64,
  description: 'Tournament description',
  conditions: 'Tournament conditions',
})

const participants = ref([
  'Shevchenko Taras Hryhorovych',
  'Kozak Volodymyr Petrovych',
  'Melnyk Mariia Ivanivna',
  'Tkachenko Artem Ihorovych',
  'Lysenko Hanna Vitaliivna',
  'Bondarenko Olena Mykolaivna',
  'Bondaryk Oksana Mykolaivna',
])

const removeParticipant = (player: string) => {
  participants.value =
    participants.value.filter(p => p !== player)
}

const handleSubmit = async () => {
  isSubmitting.value = true

  setTimeout(() => {
    isSubmitting.value = false

    router.push('/my-tournaments')
  }, 1000)
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

.edit-form__field label {
  display: block;
  margin-bottom: 10px;

  font-size: 20px;
  font-weight: 600;
}

.edit-form__input-wrapper {
  position: relative;
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