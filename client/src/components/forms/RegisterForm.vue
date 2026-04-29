<template>
  <form class="register-form" @submit.prevent="handleSubmit">
    <div class="register-form__card">
      <div class="register-form__header">
        <h1 class="register-form__title">Create Account</h1>
        <p class="register-form__subtitle">
          Join the tournament platform and start your journey to victory
        </p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="fullName">Name &amp; Surname</label>
        <div class="register-form__input-wrapper">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <path
                d="M12 12C14.7614 12 17 9.76142 17 7C17 4.23858 14.7614 2 12 2C9.23858 2 7 4.23858 7 7C7 9.76142 9.23858 12 12 12Z"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <path
                d="M4 21C4 17.6863 7.58172 15 12 15C16.4183 15 20 17.6863 20 21"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linecap="round"
              />
            </svg>
          </span>

          <input
            id="fullName"
            v-model.trim="form.fullName"
            type="text"
            class="register-form__input"
            placeholder="Enter your full name"
            maxlength="255"
            @blur="validateField('fullName')"
          />
        </div>
        <p class="register-form__error">{{ errors.fullName || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="phoneNumber">Phone</label>
        <div class="register-form__input-wrapper">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <path
                d="M5 4H9L11 9L8.5 10.5C9.57143 12.7143 11.2857 14.4286 13.5 15.5L15 13L20 15V19C20 20.1046 19.1046 21 18 21C10.268 21 4 14.732 4 7C4 5.89543 4.89543 5 6 5"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linejoin="round"
              />
            </svg>
          </span>

          <input
            id="phoneNumber"
            :value="form.phoneNumber"
            @input="handlePhoneInput"
            type="tel"
            class="register-form__input"
            placeholder="+380 XX XXX XX XX"
            @blur="validateField('phoneNumber')"
          />
          </div>
        <p class="register-form__error">{{ errors.phoneNumber || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="email">Email</label>
        <div class="register-form__input-wrapper">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <path
                d="M4 6H20V18H4V6Z"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linejoin="round"
              />
              <path
                d="M4 7L12 13L20 7"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linejoin="round"
              />
            </svg>
          </span>

          <input
            id="email"
            v-model.trim="form.email"
            type="email"
            class="register-form__input"
            placeholder="example@email.com"
            @blur="handleEmailBlur"
          />
        </div>

        <div class="register-form__hint-row">
          <p v-if="isCheckingEmail" class="register-form__hint">Checking email...</p>
          <p v-else-if="emailIsUnique === true" class="register-form__success-text">
            Email is available
          </p>
          <p v-else-if="emailIsUnique === false" class="register-form__error">
            Email is already registered
          </p>
          <p v-else class="register-form__hint"></p>
        </div>

        <p class="register-form__error">{{ errors.email || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="dateOfBirth">Date of birth</label>
        <div class="register-form__input-wrapper register-form__input-wrapper--date">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <path d="M7 2V6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              <path d="M17 2V6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              <path
                d="M4 9H20"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linecap="round"
              />
              <rect
                x="4"
                y="4"
                width="16"
                height="16"
                rx="2"
                stroke="currentColor"
                stroke-width="1.8"
              />
            </svg>
          </span>

          <input
            id="dateOfBirth"
            v-model="form.dateOfBirth"
            type="date"
            class="register-form__input register-form__input--date"
            @blur="validateField('dateOfBirth')"
          />

          <span class="register-form__date-icon">
            <svg viewBox="0 0 24 24" fill="none">
              <path d="M7 2V6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              <path d="M17 2V6" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" />
              <path
                d="M4 9H20"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linecap="round"
              />
              <rect
                x="4"
                y="4"
                width="16"
                height="16"
                rx="2"
                stroke="currentColor"
                stroke-width="1.8"
              />
            </svg>
          </span>
        </div>
        <p class="register-form__error">{{ errors.dateOfBirth || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label">Role</label>

        <div class="register-form__roles">
          <label
            class="register-form__role-card"
            :class="{ 'register-form__role-card--active': form.role === 'organizer' }"
          >
            <input
              v-model="form.role"
              type="radio"
              value="organizer"
              class="register-form__radio"
            />
            <span class="register-form__role-check"></span>

            <div class="register-form__role-icon">
              <svg viewBox="0 0 24 24" fill="none">
                <path
                  d="M8 12C9.65685 12 11 10.6569 11 9C11 7.34315 9.65685 6 8 6C6.34315 6 5 7.34315 5 9C5 10.6569 6.34315 12 8 12Z"
                  stroke="currentColor"
                  stroke-width="1.8"
                />
                <path
                  d="M16 10C17.1046 10 18 9.10457 18 8C18 6.89543 17.1046 6 16 6C14.8954 6 14 6.89543 14 8C14 9.10457 14.8954 10 16 10Z"
                  stroke="currentColor"
                  stroke-width="1.8"
                />
                <path
                  d="M3 18C3 15.7909 5.23858 14 8 14C10.7614 14 13 15.7909 13 18"
                  stroke="currentColor"
                  stroke-width="1.8"
                  stroke-linecap="round"
                />
                <path
                  d="M14 18C14 16.3431 15.7909 15 18 15"
                  stroke="currentColor"
                  stroke-width="1.8"
                  stroke-linecap="round"
                />
              </svg>
            </div>

            <div class="register-form__role-content">
              <h3>ORGANIZER</h3>
              <p>Create and manage tournaments</p>
            </div>
          </label>

          <label
            class="register-form__role-card"
            :class="{ 'register-form__role-card--active': form.role === 'player' }"
          >
            <input
              v-model="form.role"
              type="radio"
              value="player"
              class="register-form__radio"
            />
            <span class="register-form__role-check"></span>

            <div class="register-form__role-icon">
              <svg viewBox="0 0 24 24" fill="none">
                <path
                  d="M12 3L14.7812 8.63225L21 9.52786L16.5 13.9084L17.5623 20.0916L12 17.167L6.43769 20.0916L7.5 13.9084L3 9.52786L9.21885 8.63225L12 3Z"
                  stroke="currentColor"
                  stroke-width="1.8"
                  stroke-linejoin="round"
                />
              </svg>
            </div>

            <div class="register-form__role-content">
              <h3>PLAYER</h3>
              <p>Join tournaments and compete</p>
            </div>
          </label>
        </div>

        <p class="register-form__error">{{ errors.role || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="password">Password</label>
        <div class="register-form__input-wrapper">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <rect
                x="5"
                y="11"
                width="14"
                height="10"
                rx="2"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <path
                d="M8 11V8C8 5.79086 9.79086 4 12 4C14.2091 4 16 5.79086 16 8V11"
                stroke="currentColor"
                stroke-width="1.8"
              />
            </svg>
          </span>

          <input
            id="password"
            v-model="form.password"
            :type="showPassword ? 'text' : 'password'"
            class="register-form__input register-form__input--password"
            placeholder="Enter password"
            @input="handlePasswordInput"
            @blur="validateField('password')"
          />

          <button
            type="button"
            class="register-form__toggle-password"
            @click="showPassword = !showPassword"
          >
            <svg v-if="showPassword" viewBox="0 0 24 24" fill="none">
              <path
                d="M3 12C4.8 8.5 7.9 6 12 6C16.1 6 19.2 8.5 21 12C19.2 15.5 16.1 18 12 18C7.9 18 4.8 15.5 3 12Z"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.8" />
            </svg>

            <svg v-else viewBox="0 0 24 24" fill="none">
              <path
                d="M3 12C4.8 8.5 7.9 6 12 6C16.1 6 19.2 8.5 21 12C19.2 15.5 16.1 18 12 18C7.9 18 4.8 15.5 3 12Z"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.8" />
              <path
                d="M4 4L20 20"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linecap="round"
              />
            </svg>
          </button>
        </div>

        <div class="register-form__strength">
          <div class="register-form__strength-bar">
            <span class="register-form__strength-fill" :class="strengthClass"></span>
          </div>
          <p class="register-form__strength-text">
            Password strength: <strong>{{ passwordStrengthLabel }}</strong>
          </p>
        </div>

        <p class="register-form__error">{{ errors.password || '' }}</p>
      </div>

      <div class="register-form__field-group">
        <label class="register-form__label" for="confirmPassword">Confirm Password</label>
        <div class="register-form__input-wrapper">
          <span class="register-form__icon">
            <svg viewBox="0 0 24 24" fill="none">
              <rect
                x="5"
                y="11"
                width="14"
                height="10"
                rx="2"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <path
                d="M8 11V8C8 5.79086 9.79086 4 12 4C14.2091 4 16 5.79086 16 8V11"
                stroke="currentColor"
                stroke-width="1.8"
              />
            </svg>
          </span>

          <input
            id="confirmPassword"
            v-model="form.confirmPassword"
            :type="showConfirmPassword ? 'text' : 'password'"
            class="register-form__input register-form__input--password"
            placeholder="Repeat password"
            @blur="validateField('confirmPassword')"
          />

          <button
            type="button"
            class="register-form__toggle-password"
            @click="showConfirmPassword = !showConfirmPassword"
          >
            <svg v-if="showConfirmPassword" viewBox="0 0 24 24" fill="none">
              <path
                d="M3 12C4.8 8.5 7.9 6 12 6C16.1 6 19.2 8.5 21 12C19.2 15.5 16.1 18 12 18C7.9 18 4.8 15.5 3 12Z"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.8" />
            </svg>

            <svg v-else viewBox="0 0 24 24" fill="none">
              <path
                d="M3 12C4.8 8.5 7.9 6 12 6C16.1 6 19.2 8.5 21 12C19.2 15.5 16.1 18 12 18C7.9 18 4.8 15.5 3 12Z"
                stroke="currentColor"
                stroke-width="1.8"
              />
              <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.8" />
              <path
                d="M4 4L20 20"
                stroke="currentColor"
                stroke-width="1.8"
                stroke-linecap="round"
              />
            </svg>
          </button>
        </div>
        <p class="register-form__error">{{ errors.confirmPassword || '' }}</p>
      </div>

      <button type="submit" class="register-form__submit" :disabled="isSubmitting">
        {{ isSubmitting ? 'Signing Up...' : 'Sign Up' }}
      </button>

      <p class="register-form__login-text">
        Already have an account?
        <router-link to="/login" class="register-form__login-link">Log In</router-link>
      </p>

      <div class="register-form__privacy">
        <div class="register-form__privacy-icon">
          <svg viewBox="0 0 24 24" fill="none">
            <path
              d="M12 3L19 6V11C19 16 15.5 19.5 12 21C8.5 19.5 5 16 5 11V6L12 3Z"
              stroke="currentColor"
              stroke-width="1.8"
              stroke-linejoin="round"
            />
            <path
              d="M9 11L11 13L15 9"
              stroke="currentColor"
              stroke-width="1.8"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </div>
        <div>
          <p class="register-form__privacy-title">Your data is protected</p>
          <p class="register-form__privacy-text">
            We never share your information with third parties
          </p>
        </div>
      </div>

      <p v-if="submitError" class="register-form__submit-error">{{ submitError }}</p>
      <p v-if="submitSuccess" class="register-form__submit-success">
        Registration successful. Redirecting...
      </p>
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import { authService } from '../../services/authService';
import { useAuthStore } from '../../stores/authStore';

import type { IApiError, IRegisterFormValues, IRegisterRequest } from '../../types/Auth';


type FormErrors = Partial<Record<keyof IRegisterFormValues, string>>;

const router = useRouter();
const authStore = useAuthStore();

const form = reactive<IRegisterFormValues>({
  fullName: '',
  phoneNumber: '+380',
  email: '',
  dateOfBirth: '',
  role: 'organizer',
  password: '',
  confirmPassword: '',
});

const errors = reactive<FormErrors>({});

const isCheckingEmail = ref(false);
const emailIsUnique = ref<boolean | null>(null);
const isSubmitting = ref(false);
const submitError = ref('');
const submitSuccess = ref(false);

const showPassword = ref(false);
const showConfirmPassword = ref(false);


const formatPhone = (digits: string) => {
  let formatted = '+380';

  if (digits.length > 0) {
    formatted += ' ' + digits.substring(0, 2);
  }
  if (digits.length >= 3) {
    formatted += ' ' + digits.substring(2, 5);
  }
  if (digits.length >= 6) {
    formatted += ' ' + digits.substring(5, 7);
  }
  if (digits.length >= 8) {
    formatted += ' ' + digits.substring(7, 9);
  }

  return formatted;
};

const handlePhoneInput = (event: Event) => {
  const input = event.target as HTMLInputElement;

  let digits = input.value.replace(/\D/g, '');

  if (!digits.startsWith('380')) {
    digits = '380' + digits;
  }

  digits = digits.substring(3);

  digits = digits.substring(0, 9);

  form.phoneNumber = formatPhone(digits);
};


const fullNameRegex =
  /^[A-Za-zА-Яа-яІіЇїЄєҐґ'’-]+(?:\s+[A-Za-zА-Яа-яІіЇїЄєҐґ'’-]+){1,2}$/u;
const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const phoneRegex = /^\+380\s\d{2}\s\d{3}\s\d{2}\s\d{2}$/;
const backendPhoneRegex = /^\+380\d{9}$/;

const normalizePhoneNumber = (value: string) => value.replace(/\s+/g, '');

const passwordStrengthScore = computed(() => {
  let score = 0;
  const password = form.password;

  if (password.length >= 8) score += 1;
  if (/\d/.test(password)) score += 1;
  if (/[A-Z]/.test(password)) score += 1;
  if (/[!@#$%^&*(),.?":{}|<>_\-\\/[\]=+]/.test(password)) score += 1;

  return score;
});

const passwordStrengthLabel = computed(() => {
  if (!form.password) return 'Weak';
  if (passwordStrengthScore.value <= 2) return 'Weak';
  if (passwordStrengthScore.value === 3) return 'Medium';
  return 'Strong';
});

const strengthClass = computed(() => {
  if (!form.password || passwordStrengthScore.value <= 2) {
    return 'register-form__strength-fill--weak';
  }
  if (passwordStrengthScore.value === 3) {
    return 'register-form__strength-fill--medium';
  }
  return 'register-form__strength-fill--strong';
});

const getAge = (dateString: string) => {
  const today = new Date();
  const birthDate = new Date(dateString);

  let age = today.getFullYear() - birthDate.getFullYear();
  const monthDiff = today.getMonth() - birthDate.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--;
  }

  return age;
};

const validateField = (field: keyof IRegisterFormValues) => {
  switch (field) {
    case 'fullName':
      if (!form.fullName.trim()) {
        errors.fullName = 'Full name is required';
      } else if (form.fullName.length > 255) {
        errors.fullName = 'Maximum length is 255 characters';
      } else if (!fullNameRegex.test(form.fullName.trim())) {
        errors.fullName = 'Enter first name and last name';
      } else {
        errors.fullName = '';
      }
      break;

    case 'phoneNumber':
      if (!form.phoneNumber.trim()) {
        errors.phoneNumber = 'Phone number is required';
      } else if (!phoneRegex.test(form.phoneNumber.trim())) {
        errors.phoneNumber = 'Phone format must be +380 XX XXX XX XX';
      } else if (!backendPhoneRegex.test(normalizePhoneNumber(form.phoneNumber.trim()))) {
        errors.phoneNumber = 'Phone format must be +380 XX XXX XX XX';
      } else {
        errors.phoneNumber = '';
      }
      break;

    case 'email':
      if (!form.email.trim()) {
        errors.email = 'Email is required';
      } else if (!emailRegex.test(form.email.trim())) {
        errors.email = 'Invalid email format';
      } else {
        errors.email = '';
      }
      break;

    case 'dateOfBirth':
      if (!form.dateOfBirth) {
        errors.dateOfBirth = 'Date of birth is required';
      } else if (getAge(form.dateOfBirth) < 13) {
        errors.dateOfBirth = 'Registration is allowed only from the age of 13';
      } else {
        errors.dateOfBirth = '';
      }
      break;

    case 'role':
      if (!form.role) {
        errors.role = 'Role is required';
      } else {
        errors.role = '';
      }
      break;

    case 'password':
      if (!form.password) {
        errors.password = 'Password is required';
      } else if (!/^(?=.*[A-Z])(?=.*\d).{8,}$/.test(form.password)) {
        errors.password =
          'Password must contain at least 8 characters, one uppercase letter, and one digit';
      } else {
        errors.password = '';
      }
      break;

    case 'confirmPassword':
      if (!form.confirmPassword) {
        errors.confirmPassword = 'Password confirmation is required';
      } else if (form.confirmPassword !== form.password) {
        errors.confirmPassword = 'Passwords do not match';
      } else {
        errors.confirmPassword = '';
      }
      break;

    default:
      break;
  }
};

const validateForm = async () => {
  validateField('fullName');
  validateField('phoneNumber');
  validateField('email');
  validateField('dateOfBirth');
  validateField('role');
  validateField('password');
  validateField('confirmPassword');

  if (!errors.email && form.email.trim()) {
    const isUnique = await authService.checkEmailUnique(form.email.trim());
    emailIsUnique.value = isUnique;

    if (!isUnique) {
      errors.email = 'Email is already registered';
    }
  }

  return !Object.values(errors).some((value) => value);
};

const handleEmailBlur = async () => {
  validateField('email');

  if (errors.email || !form.email.trim()) {
    emailIsUnique.value = null;
    return;
  }

  isCheckingEmail.value = true;
  try {
    const isUnique = await authService.checkEmailUnique(form.email.trim());
    emailIsUnique.value = isUnique;

    if (!isUnique) {
      errors.email = 'Email is already registered';
    } else {
      errors.email = '';
    }
  } finally {
    isCheckingEmail.value = false;
  }
};

const handlePasswordInput = () => {
  validateField('password');

  if (form.confirmPassword) {
    validateField('confirmPassword');
  }
};

const handleSubmit = async () => {
  submitError.value = '';
  submitSuccess.value = false;

  const isValid = await validateForm();
  if (!isValid) return;

  isSubmitting.value = true;

  try {
    const payload: IRegisterRequest = {
      fullName: form.fullName.trim(),
      phoneNumber: normalizePhoneNumber(form.phoneNumber.trim()),
      email: form.email.trim(),
      dateOfBirth: form.dateOfBirth,
      role: form.role,
      password: form.password,
    };

    const response = await authService.register(payload);


    authStore.setAuth(response);
    submitSuccess.value = true;

    setTimeout(() => {
      router.push(authStore.getDashboardRouteByRole(response.role));
    }, 900);
  } catch (error: unknown) {

    const apiError = error as IApiError;


    if (apiError.errorCode === 'EMAIL_TAKEN') {
      submitError.value = 'Email is already registered';
      errors.email = 'Email is already registered';
    } else if (apiError.errorCode === 'VALIDATION_ERROR') {
      submitError.value = apiError.message ?? 'Validation error';
    } else {
      submitError.value = apiError.message ?? 'Server error. Please try again later.';
    }
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
.register-form {
  display: flex;
  justify-content: center;
  padding: 140px 16px 80px;
}

.register-form__card {
  width: 100%;
  max-width: 476px;
  border: 1px solid #1531ce;
  border-radius: 24px;
  background: rgba(37, 46, 53, 0.55);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  padding: 52px 52px 44px;
  color: #fffcf2;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.25);
  transform: translateZ(0);
}

.register-form__header {
  margin-bottom: 37px;
  text-align: center;
}

.register-form__title {
  margin: 0 0 12px;
  font-size: 40px;
  font-weight: 800;
  line-height: 1.15;
}

.register-form__subtitle {
  margin: 0;
  color: #fffcf2;
  font-size: 16px;
  line-height: 1.5;
  opacity: 0.9;
}

.register-form__field-group {
  margin-bottom: 37px;
}

.register-form__field-group:last-of-type {
  margin-bottom: 28px;
}

.register-form__label {
  display: block;
  margin-bottom: 10px;
  font-size: 20px;
  font-weight: 600;
}

.register-form__input-wrapper {
  position: relative;
}

.register-form__input-wrapper--date {
  position: relative;
}

.register-form__icon {
  position: absolute;
  top: 50%;
  left: 16px;
  width: 24px;
  height: 24px;
  color: #ffffff;
  transform: translateY(-50%);
  opacity: 1;
  pointer-events: none;
  flex-shrink: 0;
}

.register-form__icon svg {
  width: 24px;
  height: 24px;
  stroke: #ffffff;
  fill: none;
}

.register-form__input {
  width: 100%;
  height: 48px;
  border: 1px solid #1531ce;
  border-radius: 12px;
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
  padding: 0 48px 0 54px;
  font-size: 13px;
  outline: none;
  appearance: none;
  -webkit-appearance: none;
  background-clip: padding-box;
}

.register-form__input:focus,
.register-form__input:active {
  background: rgba(21, 49, 206, 0.47);
  border-color: #1531ce;
  box-shadow: none;
}

.register-form__input[type='password']::-ms-reveal,
.register-form__input[type='password']::-ms-clear {
  display: none;
}

.register-form__input[type='password']::-webkit-credentials-auto-fill-button,
.register-form__input[type='password']::-webkit-textfield-decoration-container {
  display: none !important;
}

.register-form__input:-webkit-autofill,
.register-form__input:-webkit-autofill:hover,
.register-form__input:-webkit-autofill:focus,
.register-form__input:-webkit-autofill:active {
  -webkit-text-fill-color: #fffcf2;
  -webkit-box-shadow: 0 0 0 1000px rgba(21, 49, 206, 0.47) inset;
  transition: background-color 9999s ease-in-out 0s;
  border: 1px solid #1531ce;
}

.register-form__input::placeholder {
  color: rgba(255, 252, 242, 0.7);
  font-size: 13px;
}

.register-form__input--date {
  padding-right: 48px;
  color-scheme: dark;
}

.register-form__input--date::-webkit-calendar-picker-indicator {
  opacity: 0;
  position: absolute;
  right: 14px;
  width: 24px;
  height: 24px;
  cursor: pointer;
}

.register-form__date-icon {
  position: absolute;
  top: 50%;
  right: 14px;
  width: 24px;
  height: 24px;
  color: #ffffff;
  transform: translateY(-50%);
  pointer-events: none;
  display: flex;
  align-items: center;
  justify-content: center;
}

.register-form__date-icon svg {
  width: 24px;
  height: 24px;
  stroke: #ffffff;
  fill: none;
}

.register-form__input--password {
  padding-right: 54px;
}

.register-form__toggle-password {
  position: absolute;
  top: 50%;
  right: 14px;
  width: 24px;
  height: 24px;
  border: none;
  background: transparent;
  color: #ffffff;
  transform: translateY(-50%);
  cursor: pointer;
  opacity: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
}

.register-form__toggle-password svg {
  width: 24px;
  height: 24px;
  stroke: #ffffff;
  fill: none;
  flex-shrink: 0;
}

.register-form__hint-row {
  min-height: 18px;
  margin-top: 6px;
}

.register-form__hint {
  margin: 0;
  min-height: 17px;
  font-size: 12px;
  color: #fffcf2;
  opacity: 0.8;
}

.register-form__error {
  margin: 6px 0 0;
  min-height: 17px;
  font-size: 12px;
  color: #ff6b6b;
  line-height: 1.4;
}

.register-form__success-text {
  margin: 0;
  min-height: 17px;
  font-size: 12px;
  color: #84c082;
}

.register-form__roles {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

.register-form__role-card {
  position: relative;
  display: flex;
  flex-direction: column;
  gap: 10px;
  min-height: 140px;
  border: 1px solid #1531ce;
  border-radius: 18px;
  background: rgba(37, 46, 53, 0.88);
  padding: 18px 16px 16px;
  cursor: pointer;
}

.register-form__role-card--active {
  box-shadow: 0 0 0 1px #1531ce inset;
}

.register-form__radio {
  position: absolute;
  opacity: 0;
  pointer-events: none;
}

.register-form__role-check {
  position: absolute;
  top: 14px;
  right: 14px;
  width: 18px;
  height: 18px;
  border: 2px solid #fffcf2;
  border-radius: 50%;
  background: #fffcf2;
}

.register-form__role-card--active .register-form__role-check {
  border-color: #1531ce;
  background: #fffcf2;
  box-shadow: inset 0 0 0 5px #1531ce;
}

.register-form__role-icon {
  width: 24px;
  height: 24px;
  color: #fffcf2;
}

.register-form__role-icon svg {
  width: 100%;
  height: 100%;
}

.register-form__role-content h3 {
  margin: 0 0 10px;
  font-size: 13px;
  font-weight: 800;
}

.register-form__role-content p {
  margin: 0;
  color: #fffcf2;
  font-size: 13px;
  line-height: 1.4;
  opacity: 0.9;
}

.register-form__strength {
  margin-top: 10px;
}

.register-form__strength-bar {
  width: 100%;
  height: 8px;
  border-radius: 999px;
  background: rgba(255, 252, 242, 0.15);
  overflow: hidden;
}

.register-form__strength-fill {
  display: block;
  height: 100%;
  width: 35%;
  border-radius: 999px;
  transition: width 0.2s ease, background 0.2s ease;
}

.register-form__strength-fill--weak {
  width: 35%;
  background: #ff6b6b;
}

.register-form__strength-fill--medium {
  width: 68%;
  background: #ff9800;
}

.register-form__strength-fill--strong {
  width: 100%;
  background: #84c082;
}

.register-form__strength-text {
  margin: 8px 0 0;
  font-size: 12px;
  color: #fffcf2;
  opacity: 0.9;
}

.register-form__submit {
  width: 100%;
  height: 46px;
  border: 1px solid #ff9800;
  border-radius: 12px;
  background: #ff9800;
  color: #fffcf2;
  font-size: 16px;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.2s ease, color 0.2s ease, border-color 0.2s ease;
}

.register-form__submit:hover {
  background: #ff9800;
  color: #fffcf2;
}

.register-form__submit:active {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
}

.register-form__submit:focus-visible {
  background: transparent;
  color: #ff9800;
  border-color: #ff9800;
  outline: none;
}

.register-form__submit:disabled {
  opacity: 0.75;
  cursor: not-allowed;
}

.register-form__login-text {
  margin: 26px 0 20px;
  text-align: center;
  font-size: 15px;
  color: #fffcf2;
}

.register-form__login-link {
  color: #ff9800;
  font-weight: 700;
  text-decoration: underline;
  text-underline-offset: 3px;
}

.register-form__privacy {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
  border-top: 1px solid rgba(21, 49, 206, 0.45);
  padding-top: 18px;
  text-align: center;
}

.register-form__privacy-icon {
  width: 20px;
  height: 20px;
  color: #fffcf2;
  flex-shrink: 0;
}

.register-form__privacy-icon svg {
  width: 100%;
  height: 100%;
}

.register-form__privacy-title {
  margin: 0 0 10px;
  font-size: 15px;
  font-weight: 600;
}

.register-form__privacy-text {
  margin: 0;
  font-size: 11px;
  opacity: 0.85;
}

.register-form__submit-error,
.register-form__submit-success {
  margin: 18px 0 0;
  border-radius: 12px;
  padding: 14px 16px;
  font-size: 14px;
  line-height: 1.5;
}

.register-form__submit-error {
  border: 1px solid rgba(255, 107, 107, 0.4);
  background: rgba(255, 107, 107, 0.12);
  color: #ff6b6b;
}

.register-form__submit-success {
  border: 1px solid rgba(132, 192, 130, 0.4);
  background: rgba(132, 192, 130, 0.12);
  color: #84c082;
}

@media (max-width: 640px) {
  .register-form {
    padding: 120px 12px 56px;
  }

  .register-form__card {
    padding: 32px 20px 28px;
    border-radius: 20px;
  }

  .register-form__title {
    font-size: 30px;
  }

  .register-form__subtitle {
    font-size: 14px;
  }

  .register-form__label {
    font-size: 18px;
  }

  .register-form__roles {
    grid-template-columns: 1fr;
  }
}
</style>