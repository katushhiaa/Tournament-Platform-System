<template>
  <form class="create-tournament-form" @submit.prevent="handleSubmit">
    <div class="create-tournament-form__top-card">
      <div class="create-tournament-form__upload">
                <button
          type="button"
          class="create-tournament-form__upload"
          @click="handleBannerClick"
        >
          <input
            ref="bannerInput"
            type="file"
            accept="image/png,image/jpeg"
            class="create-tournament-form__file-input"
            @change="handleBannerChange"
          />

          <img :src="uploadBannerIcon" alt="" class="create-tournament-form__upload-icon" />

          <p class="create-tournament-form__upload-title">
            {{ bannerFileName || 'Upload Banner' }}
          </p>
          <p class="create-tournament-form__upload-text">PNG, JPG up to 5MB</p>
          <p class="create-tournament-form__upload-text">Recommended 16:9</p>
        </button>
        
      </div>

      <div class="create-tournament-form__grid">
        <div class="create-tournament-form__field">
          <label for="title">Tournament Name</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{ 'create-tournament-form__input-wrapper--error': errors.title }"
          >
            <img :src="nameIcon" alt="" class="create-tournament-form__icon" />
            <input
              id="title"
              ref="titleInput"
              v-model.trim="form.title"
              type="text"
              placeholder="Enter tournament name"
              maxlength="255"
              @blur="validateField('title')"
              @input="validateField('title')"
            />
          </div>
          <p class="create-tournament-form__error">{{ errors.title || '' }}</p>
        </div>

        <div class="create-tournament-form__field">
          <label for="startDate">Date start</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{ 'create-tournament-form__input-wrapper--error': errors.startDate }"
          >
            <img :src="dateIcon" alt="" class="create-tournament-form__icon" />
            <input
              id="startDate"
              ref="startDateInput"
              v-model="form.startDate"
              type="datetime-local"
              @blur="validateField('startDate')"
              @change="validateDateFields"
            />
          </div>
          <p class="create-tournament-form__error">{{ errors.startDate || '' }}</p>
        </div>

        <div class="create-tournament-form__field">
          <label for="sport">Sport Type</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{ 'create-tournament-form__input-wrapper--error': errors.sport }"
          >
            <img :src="sportIcon" alt="" class="create-tournament-form__icon" />

            <select
              id="sport"
              ref="sportInput"
              v-model="form.sport"
              class="create-tournament-form__select"
              @blur="validateField('sport')"
              @change="validateField('sport')"
            >
              <option value="" disabled>Select a game</option>
              <option v-for="theme in themes" :key="theme.id" :value="theme.id">
                {{ theme.name }}
              </option>
            </select>

            <img :src="dropdownIcon" alt="" class="create-tournament-form__dropdown-icon" />
          </div>
          <p class="create-tournament-form__error">{{ errors.sport || '' }}</p>
        </div>

        <div class="create-tournament-form__field">
          <label for="endDate">Date end</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{ 'create-tournament-form__input-wrapper--error': errors.endDate }"
          >
            <img :src="dateIcon" alt="" class="create-tournament-form__icon" />
            <input
              id="endDate"
              ref="endDateInput"
              v-model="form.endDate"
              type="datetime-local"
              @blur="validateField('endDate')"
              @change="validateDateFields"
            />
          </div>
          <p class="create-tournament-form__error">{{ errors.endDate || '' }}</p>
        </div>

        <div class="create-tournament-form__field">
          <label for="maxParticipants">Participants max count</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{ 'create-tournament-form__input-wrapper--error': errors.maxParticipants }"
          >
            <img :src="peopleIcon" alt="" class="create-tournament-form__icon" />
            <input
              id="maxParticipants"
              ref="maxParticipantsInput"
              v-model.number="form.maxParticipants"
              type="number"
              min="2"
              placeholder="e.g. 32"
              @blur="validateField('maxParticipants')"
              @input="validateField('maxParticipants')"
            />
          </div>
          <p class="create-tournament-form__error">{{ errors.maxParticipants || '' }}</p>
        </div>

        <div class="create-tournament-form__field">
          <label for="registrationCloseDate">End of Registration</label>
          <div
            class="create-tournament-form__input-wrapper"
            :class="{
              'create-tournament-form__input-wrapper--error': errors.registrationCloseDate,
            }"
          >
            <img :src="timeIcon" alt="" class="create-tournament-form__icon" />
            <input
              id="registrationCloseDate"
              ref="registrationCloseDateInput"
              v-model="form.registrationCloseDate"
              type="datetime-local"
              @blur="validateField('registrationCloseDate')"
              @change="validateDateFields"
            />
          </div>
          <p class="create-tournament-form__error">
            {{ errors.registrationCloseDate || '' }}
          </p>
        </div>
      </div>
    </div>

    <div class="create-tournament-form__card">
      <label for="description">Description</label>
      <textarea
        id="description"
        v-model.trim="form.description"
        placeholder="Tell players about your tournament, format, prize pool, and what to expect..."
      ></textarea>
      <p class="create-tournament-form__hint">Markdown is supported</p>
    </div>

    <div class="create-tournament-form__card">
      <label for="conditions">Conditions</label>
      <textarea
        id="conditions"
        v-model.trim="form.conditions"
        placeholder="List the rules, requirements, and conditions for participants..."
      ></textarea>
      <p class="create-tournament-form__hint">
        Examples: Age limit, allowed regions, rules, anti-cheat, connection requirements, etc.
      </p>
    </div>

    <p v-if="toastMessage" class="create-tournament-form__toast">{{ toastMessage }}</p>
    <p v-if="successMessage" class="create-tournament-form__success">{{ successMessage }}</p>

    <div class="create-tournament-form__actions">
      <button
        type="submit"
        class="create-tournament-form__submit"
        :disabled="isSubmitting"
      >
        <img :src="createIcon" alt="" class="create-tournament-form__button-icon" />
        {{ isSubmitting ? 'Creating...' : 'Create tournament' }}
      </button>

      <button
        type="button"
        class="create-tournament-form__cancel"
        @click="router.push('/my-tournaments')"
      >
        Cancel
      </button>
    </div>

  </form>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import uploadBannerIcon from '../../assets/icons/Upload Banner.png';
import nameIcon from '../../assets/icons/Name.png';
import sportIcon from '../../assets/icons/Sport.png';
import dropdownIcon from '../../assets/icons/drop_down_list.png';
import peopleIcon from '../../assets/icons/people.png';
import dateIcon from '../../assets/icons/date.png';
import timeIcon from '../../assets/icons/time.png';
import createIcon from '../../assets/icons/Create.png';
import { tournamentService } from '../../services/tournamentService';
import type { IThemeOption, ITournamentCreate, ITournamentResponse } from '../../types/Tournament';
import type { IApiError } from '../../types/Auth';

const emit = defineEmits<{
  created: [tournament: ITournamentResponse];
}>();

type CreateTournamentFormValues = {
  title: string;
  sport: string;
  startDate: string;
  endDate: string;
  registrationCloseDate: string;
  maxParticipants: number | null;
  description: string;
  conditions: string;
};

const bannerInput = ref<HTMLInputElement | null>(null);
const bannerFileName = ref('');

const handleBannerClick = () => {
  bannerInput.value?.click();
};

const handleBannerChange = (event: Event) => {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (!file) return;

  bannerFileName.value = file.name;
};

type FormErrors = Partial<Record<keyof CreateTournamentFormValues, string>>;

const router = useRouter();

const minTeams = 2;

const form = reactive<CreateTournamentFormValues>({
  title: '',
  sport: '',
  startDate: '',
  endDate: '',
  registrationCloseDate: '',
  maxParticipants: null,
  description: '',
  conditions: '',
});

const errors = reactive<FormErrors>({});
const themes = ref<IThemeOption[]>([]);
const isSubmitting = ref(false);
const toastMessage = ref('');
const successMessage = ref('');

const titleInput = ref<HTMLInputElement | null>(null);
const sportInput = ref<HTMLSelectElement | null>(null);
const startDateInput = ref<HTMLInputElement | null>(null);
const endDateInput = ref<HTMLInputElement | null>(null);
const registrationCloseDateInput = ref<HTMLInputElement | null>(null);
const maxParticipantsInput = ref<HTMLInputElement | null>(null);

const toIsoDate = (value: string) => new Date(value).toISOString();

const focusFirstError = () => {
  if (errors.title) return titleInput.value?.focus();
  if (errors.sport) return sportInput.value?.focus();
  if (errors.startDate) return startDateInput.value?.focus();
  if (errors.endDate) return endDateInput.value?.focus();
  if (errors.registrationCloseDate) return registrationCloseDateInput.value?.focus();
  if (errors.maxParticipants) return maxParticipantsInput.value?.focus();

  return undefined;
};

const validateField = (field: keyof CreateTournamentFormValues) => {
  switch (field) {
    case 'title':
      if (!form.title.trim()) {
        errors.title = 'Tournament name is required';
      } else if (form.title.length > 255) {
        errors.title = 'Tournament name must be less than 255 characters';
      } else {
        errors.title = '';
      }
      break;

    case 'sport':
      errors.sport = form.sport ? '' : 'Sport type is required';
      break;

    case 'startDate':
      errors.startDate = form.startDate ? '' : 'Start date is required';
      break;

    case 'endDate':
      if (!form.endDate) {
        errors.endDate = 'End date is required';
      } else if (form.startDate && new Date(form.endDate) < new Date(form.startDate)) {
        errors.endDate = 'Date end must be greater than or equal to date start';
      } else {
        errors.endDate = '';
      }
      break;

    case 'registrationCloseDate':
      if (!form.registrationCloseDate) {
        errors.registrationCloseDate = 'End of registration is required';
      } else if (
        form.startDate &&
        new Date(form.registrationCloseDate) > new Date(form.startDate)
      ) {
        errors.registrationCloseDate =
          'End of registration must be less than or equal to date start';
      } else {
        errors.registrationCloseDate = '';
      }
      break;

    case 'maxParticipants':
      if (!form.maxParticipants) {
        errors.maxParticipants = 'Participants max count is required';
      } else if (form.maxParticipants < minTeams) {
        errors.maxParticipants = `Participants max count must be at least ${minTeams}`;
      } else {
        errors.maxParticipants = '';
      }
      break;

    default:
      break;
  }
};

const validateDateFields = () => {
  validateField('startDate');
  validateField('endDate');
  validateField('registrationCloseDate');
};

const validateForm = () => {
  validateField('title');
  validateField('sport');
  validateDateFields();
  validateField('maxParticipants');

  return !Object.values(errors).some(Boolean);
};

const handleSubmit = async () => {
  toastMessage.value = '';
  successMessage.value = '';

  const isValid = validateForm();

  if (!isValid) {
    focusFirstError();
    return;
  }

  isSubmitting.value = true;

  try {
    const payload: ITournamentCreate = {
      title: form.title.trim(),
      description: form.description.trim() || null,
      conditions: form.conditions.trim() || null,
      startDate: toIsoDate(form.startDate),
      endDate: toIsoDate(form.endDate),
      registrationCloseDate: toIsoDate(form.registrationCloseDate),
      sport: form.sport,
      maxParticipants: Number(form.maxParticipants),
    };

    const response = await tournamentService.createTournament(payload);

    successMessage.value = 'Tournament created successfully';
    emit('created', response);
  } catch (error: unknown) {
    const apiError = error as IApiError;

    if (apiError.errorCode === 'CONFLICT') {
      toastMessage.value = apiError.message || 'Conflict. Please check tournament data.';
    } else if (apiError.errorCode === 'VALIDATION_ERROR') {
      toastMessage.value = apiError.message || 'Validation error';
    } else {
      toastMessage.value = apiError.message || 'Server error. Please try again later.';
    }
  } finally {
    isSubmitting.value = false;
  }
};

onMounted(async () => {
  themes.value = await tournamentService.getThemes();
});
</script>

<style scoped>
.create-tournament-form {
  width: 1109px;
  max-width: calc(100% - 32px);
  margin: 0 auto;
  color: #fffcf2;
}

.create-tournament-form__top-card {
  width: 1106px;
  max-width: 100%;
  min-height: 430px;
  display: grid;
  grid-template-columns: 268px 1fr;
  gap: 34px;
  padding: 37px 60px;
  border-radius: 18px;
  background: rgba(37, 46, 53, 0.95);
}

.create-tournament-form__upload {
  width: 268px;
  height: 320px;
  border: 1px dashed #1531ce;
  border-radius: 14px;
  background: rgba(21, 49, 206, 0.18);
  color: #fffcf2;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  cursor: pointer;
}

.create-tournament-form__file-input {
  display: none;
}

.create-tournament-form__icon,
.create-tournament-form__dropdown-icon,
.create-tournament-form__button-icon {
  filter: brightness(0) invert(1);
}
.create-tournament-form__upload-icon {
  width: 64px;
  height: 64px;
  object-fit: contain;
  margin-bottom: 22px;
}

.create-tournament-form__upload-title {
  margin: 0 0 10px;
  font-size: 16px;
  font-weight: 700;
}

.create-tournament-form__upload-text {
  margin: 0;
  font-size: 14px;
  opacity: 0.9;
}

.create-tournament-form__dropdown-icon {
  position: absolute;
  top: 50%;
  right: 14px;
  width: 24px;
  height: 24px;
  transform: translateY(-50%);
  pointer-events: none;
  object-fit: contain;
}

.create-tournament-form__input-wrapper input[type='datetime-local']::-webkit-calendar-picker-indicator {
  opacity: 0;
  position: absolute;
  right: 12px;
  width: 24px;
  height: 24px;
  cursor: pointer;
}

.create-tournament-form__grid {
  display: grid;
  grid-template-columns: repeat(2, 334px);
  gap: 22px 30px;
  align-content: center;
}

.create-tournament-form__field label,
.create-tournament-form__card label {
  display: block;
  margin-bottom: 10px;
  font-size: 20px;
  font-weight: 600;
}

.create-tournament-form__input-wrapper {
  position: relative;
  width: 334px;
}

.create-tournament-form__input-wrapper input,
.create-tournament-form__input-wrapper select {
  width: 334px;
  height: 44px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
  padding: 0 44px 0 48px;
  font-size: 13px;
  outline: none;
}

.create-tournament-form__select {
  appearance: none;
  -webkit-appearance: none;
  -moz-appearance: none;
  background-color: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
}

.create-tournament-form__select::-ms-expand {
  display: none;
}

.create-tournament-form__select option {
  background: #1531ce;
  color: #fffcf2;
}

.create-tournament-form__input-wrapper input::placeholder,
.create-tournament-form__card textarea::placeholder {
  color: rgba(255, 252, 242, 0.65);
  font-size: 13px;
}

.create-tournament-form__icon {
  position: absolute;
  top: 50%;
  left: 14px;
  width: 24px;
  height: 24px;
  transform: translateY(-50%);
  object-fit: contain;
}

.create-tournament-form__dropdown-icon {
  position: absolute;
  top: 50%;
  right: 14px;
  width: 24px;
  height: 24px;
  transform: translateY(-50%);
  pointer-events: none;
}

.create-tournament-form__card {
  width: 1109px;
  max-width: 100%;
  min-height: 336px;
  margin-top: 28px;
  padding: 34px 64px;
  border-radius: 18px;
  background: rgba(37, 46, 53, 0.95);
}

.create-tournament-form__card textarea {
  width: 942px;
  max-width: 100%;
  min-height: 190px;
  border: 1px solid #1531ce;
  border-radius: 14px;
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
  resize: vertical;
  padding: 18px 20px;
  font-size: 13px;
  outline: none;
}

.create-tournament-form__hint {
  margin: 12px 0 0;
  font-size: 13px;
  color: rgba(255, 252, 242, 0.72);
}

.create-tournament-form__actions {
  display: flex;
  justify-content: flex-end;
  gap: 22px;
  margin: 28px 0 0;
}

.create-tournament-form__submit {
  width: 281px;
  height: 46px;
  border: 1px solid #ff9800;
  border-radius: 10px;
  background: #ff9800;
  color: #fffcf2;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.create-tournament-form__button-icon {
  width: 24px;
  height: 22px;
  object-fit: contain;
}

.create-tournament-form__cancel {
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

.create-tournament-form__input-wrapper--error input,
.create-tournament-form__input-wrapper--error select {
  border-color: #ff6b6b;
}

.create-tournament-form__error {
  min-height: 17px;
  margin: 6px 0 0;
  color: #ff6b6b;
  font-size: 12px;
}

@media (max-width: 1100px) {
  .create-tournament-form {
    width: calc(100% - 32px);
  }

  .create-tournament-form__top-card {
    grid-template-columns: 1fr;
    width: 100%;
  }

  .create-tournament-form__upload {
    width: 100%;
  }

  .create-tournament-form__grid {
    grid-template-columns: 1fr;
  }

  .create-tournament-form__input-wrapper,
  .create-tournament-form__input-wrapper input,
  .create-tournament-form__input-wrapper select {
    width: 100%;
  }
}
</style>