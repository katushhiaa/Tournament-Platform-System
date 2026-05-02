<template>
  <form class="login-form" @submit.prevent="handleSubmit">
    <div class="login-form__card">
      <div class="login-form__header">
        <h1 class="login-form__title">Log In</h1>
        <p class="login-form__subtitle">Log in to continue your journey</p>
      </div>

      <div class="login-form__field-group">
        <label class="login-form__label" for="email">Email</label>

        <div
          class="login-form__input-wrapper"
          :class="{ 'login-form__input-wrapper--error': errors.email }"
        >
          <span class="login-form__icon">
            <img :src="emailIcon" alt="" />
          </span>

          <input
            id="email"
            ref="emailInput"
            v-model.trim="form.email"
            type="email"
            class="login-form__input"
            placeholder="Enter your email"
            autocomplete="email"
            @blur="validateField('email')"
            @input="validateField('email')"
          />
        </div>

        <p class="login-form__error">{{ errors.email || '' }}</p>
      </div>

      <div class="login-form__field-group">
        <label class="login-form__label" for="password">Password</label>

        <div
          class="login-form__input-wrapper"
          :class="{ 'login-form__input-wrapper--error': errors.password }"
        >
          <span class="login-form__icon">
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
            ref="passwordInput"
            v-model="form.password"
            :type="showPassword ? 'text' : 'password'"
            class="login-form__input login-form__input--password"
            placeholder="Enter your password"
            autocomplete="current-password"
            @blur="validateField('password')"
            @input="validateField('password')"
          />

          <button
            type="button"
            class="login-form__toggle-password"
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

        <p class="login-form__error">{{ errors.password || '' }}</p>
      </div>

      <div class="login-form__options">
        <label class="login-form__remember">
          <input v-model="form.rememberMe" type="checkbox" />
          <span>Remember me</span>
        </label>

        <span class="login-form__forgot">Forgot password?</span>
      </div>

      <button
        type="submit"
        class="login-form__submit"
        :disabled="isSubmitting || isLoginBlocked"
      >
        {{ isSubmitting ? 'Logging In...' : 'Log In' }}
      </button>

      <div class="login-form__signup-block">
        <div class="login-form__divider">
          <span></span>
          <strong>New to ZVYTIAHA?</strong>
          <span></span>
        </div>

        <p>Sign up and start competing!</p>

        <router-link to="/register" class="login-form__create-account">
          Create Account
        </router-link>
      </div>

      <div class="login-form__privacy">
        <div class="login-form__privacy-icon">
          <img :src="protectedIcon" alt="" />
        </div>

        <p class="login-form__privacy-title">Your data is protected</p>
        <p class="login-form__privacy-text">
          We never share your information with third parties
        </p>
      </div>

      <p v-if="toastMessage" class="login-form__toast">{{ toastMessage }}</p>
      <p v-if="submitError" class="login-form__submit-error">{{ submitError }}</p>
    </div>
  </form>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../../stores/authStore';
import type { ILoginFormValues } from '../../types/Auth';

import emailIcon from '../../assets/icons/System Icons.png';
import protectedIcon from '../../assets/icons/Protected.png';

type FormErrors = Partial<Record<keyof ILoginFormValues, string>>;

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const isLoginBlocked = ref(false);

const form = reactive<ILoginFormValues>({
  email: localStorage.getItem('login_email') ?? '',
  password: '',
  rememberMe: true,
});

const errors = reactive<FormErrors>({});
const isSubmitting = ref(false);
const submitError = ref('');
const toastMessage = ref('');
const showPassword = ref(false);

const emailInput = ref<HTMLInputElement | null>(null);
const passwordInput = ref<HTMLInputElement | null>(null);

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const focusFirstError = () => {
  if (errors.email) {
    emailInput.value?.focus();
    return;
  }

  if (errors.password) {
    passwordInput.value?.focus();
  }
};

const applyBackendErrors = (fieldErrors?: Record<string, string>) => {
  if (!fieldErrors) return;

  if (fieldErrors.email) errors.email = fieldErrors.email;
  if (fieldErrors.password) errors.password = fieldErrors.password;

  focusFirstError();
};

const validateField = (field: keyof ILoginFormValues) => {
  switch (field) {
    case 'email':
      if (!form.email.trim()) {
        errors.email = 'Email is required';
      } else if (!emailRegex.test(form.email.trim())) {
        errors.email = 'Invalid email format';
      } else {
        errors.email = '';
      }
      localStorage.setItem('login_email', form.email);
      break;

    case 'password':
      if (!form.password) {
        errors.password = 'Password is required';
      } else {
        errors.password = '';
      }
      break;

    default:
      break;
  }
};

const validateForm = () => {
  validateField('email');
  validateField('password');

  return !Object.values(errors).some(Boolean);
};

const handleSubmit = async () => {
  submitError.value = '';
  toastMessage.value = '';

  const isValid = validateForm();

  if (!isValid) {
    focusFirstError();
    return;
  }

  isSubmitting.value = true;

  try {
    const response = await authStore.login({
      email: form.email,
      password: form.password,
      rememberMe: form.rememberMe,
    });

    const redirectPath =
      typeof route.query.redirect === 'string'
        ? route.query.redirect
        : authStore.getDashboardRouteByRole(response.role);

    await router.push(redirectPath);
  } catch (error: unknown) {
    const apiError = error as {
      errorCode?: string;
      message?: string;
      fieldErrors?: Record<string, string>;
    };

    if (apiError.errorCode === 'INVALID_CREDENTIALS') {
      submitError.value = 'Invalid email or password';
    } else if (apiError.errorCode === 'TOO_MANY_ATTEMPTS') {
      submitError.value =
        apiError.message ?? 'Too many failed login attempts. Try again later.';
      isLoginBlocked.value = true;
      window.setTimeout(
        () => {
          isLoginBlocked.value = false;
          submitError.value = '';
        },
        5 * 60 * 1000,
      );
    } else if (apiError.errorCode === 'ACCOUNT_LOCKED') {
      submitError.value = 'Account is locked. Please contact support.';
    } else if (apiError.errorCode === 'VALIDATION_ERROR') {
      applyBackendErrors(apiError.fieldErrors);
      submitError.value = apiError.message ?? 'Validation error';
    } else {
      toastMessage.value = 'Помилка сервера';
      submitError.value = apiError.message ?? 'Server error. Please try again later.';
    }
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
.login-form__input:-webkit-autofill,
.login-form__input:-webkit-autofill:hover,
.login-form__input:-webkit-autofill:focus {
  -webkit-box-shadow: 0 0 0px 1000px rgba(21, 49, 206, 0.47) inset !important;
  -webkit-text-fill-color: #fffcf2 !important;
  caret-color: #fffcf2;
  transition: background-color 5000s ease-in-out 0s;
}

.login-form {
  display: flex;
  justify-content: center;
  padding: 0 16px 80px;
}

.login-form__card {
  width: 100%;
  max-width: 543px;
  border: 1px solid #1531ce;
  border-radius: 18px;
  background: rgba(37, 46, 53, 0.21);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  padding: 43px 55px 41px;
  color: #fffcf2;
}

.login-form__header {
  margin-bottom: 42px;
  text-align: center;
}

.login-form__title {
  margin: 0 0 20px;
  color: #fffcf2;
  font-size: 40px;
  font-weight: 800;
  line-height: 1.15;
}

.login-form__subtitle {
  margin: 0;
  color: #fffcf2;
  font-size: 16px;
  line-height: 1.4;
  opacity: 0.82;
}

.login-form__field-group {
  margin-bottom: 31px;
}

.login-form__label {
  display: block;
  margin-bottom: 13px;
  color: #fffcf2;
  font-size: 20px;
  font-weight: 700;
}

.login-form__input-wrapper {
  position: relative;
  width: 433px;
  max-width: 100%;
}

.login-form__input-wrapper--error .login-form__input {
  border-color: #ff6b6b;
}

.login-form__icon {
  position: absolute;
  top: 50%;
  left: 16px;
  width: 24px;
  height: 24px;
  color: #fffcf2;
  transform: translateY(-50%);
  display: flex;
  align-items: center;
  justify-content: center;
  pointer-events: none;
}

.login-form__icon img,
.login-form__icon svg {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

.login-form__input {
  width: 433px;
  max-width: 100%;
  height: 43px;
  border: 1px solid #1531ce;
  border-radius: 12px;
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
  padding: 0 52px 0 50px;
  font-size: 13px;
  outline: none;
}

.login-form__input:focus {
  background: rgba(21, 49, 206, 0.47);
  color: #fffcf2;
}

.login-form__input::placeholder {
  color: rgba(255, 252, 242, 0.65);
  font-size: 13px;
}

.login-form__input--password {
  padding-right: 54px;
}

.login-form__toggle-password {
  position: absolute;
  top: 50%;
  right: 15px;
  width: 24px;
  height: 24px;
  border: none;
  background: transparent;
  color: #fffcf2;
  transform: translateY(-50%);
  cursor: pointer;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-form__toggle-password svg {
  width: 24px;
  height: 24px;
}

.login-form__input::-ms-reveal,
.login-form__input::-ms-clear {
  display: none;
}

.login-form__input[type='password']::-webkit-credentials-auto-fill-button,
.login-form__input[type='password']::-webkit-textfield-decoration-container {
  display: none !important;
}

input[type='password']::-webkit-credentials-auto-fill-button {
  visibility: hidden;
  display: none !important;
}

.login-form__error {
  margin: 6px 0 0;
  min-height: 17px;
  color: #ff6b6b;
  font-size: 12px;
  line-height: 1.4;
}

.login-form__options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 433px;
  max-width: 100%;
  margin: 0 0 37px;
}

.login-form__remember {
  display: flex;
  align-items: center;
  gap: 10px;
  color: #fffcf2;
  font-size: 15px;
  font-weight: 500;
}

.login-form__remember input {
  width: 20px;
  height: 20px;
  accent-color: #1531ce;
  cursor: pointer;
}

.login-form__forgot {
  color: #fffcf2;
  font-size: 15px;
  font-weight: 600;
  text-decoration: underline;
  text-underline-offset: 3px;
  cursor: default;
}

.login-form__submit {
  width: 433px;
  max-width: 100%;
  height: 40px;
  border: 1px solid #ff9800;
  border-radius: 12px;
  background: #ff9800;
  color: #fffcf2;
  font-size: 15px;
  font-weight: 700;
  cursor: pointer;
}

.login-form__submit:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.login-form__signup-block {
  width: 433px;
  max-width: 100%;
  margin-top: 37px;
  text-align: center;
}

.login-form__divider {
  display: flex;
  align-items: center;
  gap: 10px;
  color: rgba(255, 252, 242, 0.65);
}

.login-form__divider strong {
  color: rgba(255, 252, 242, 0.65);
  font-size: 16px;
  font-weight: 700;
  white-space: nowrap;
}

.login-form__divider span {
  flex: 1;
  height: 1px;
  background: rgba(255, 252, 242, 0.4);
}

.login-form__signup-block p {
  margin: 20px 0 37px;
  color: rgba(255, 252, 242, 0.82);
  font-size: 14px;
  line-height: 1.4;
}

.login-form__create-account {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 433px;
  max-width: 100%;
  height: 40px;
  border: 1px solid #1531ce;
  border-radius: 10px;
  background: rgba(21, 49, 206, 0.22);
  color: #fffcf2;
  font-size: 15px;
  font-weight: 700;
  text-decoration: none;
}

.login-form__privacy {
  margin-top: 36px;
  text-align: center;
}

.login-form__privacy-icon {
  width: 24px;
  height: 24px;
  margin: 0 auto 8px;
}

.login-form__privacy-icon img {
  width: 24px;
  height: 24px;
  object-fit: contain;
}

.login-form__privacy-title {
  margin: 0 0 4px;
  color: #fffcf2;
  font-size: 15px;
  font-weight: 700;
}

.login-form__privacy-text {
  margin: 0;
  color: #fffcf2;
  font-size: 11px;
  line-height: 1.4;
  opacity: 0.9;
}

.login-form__toast,
.login-form__submit-error {
  width: 433px;
  max-width: 100%;
  margin: 18px 0 0;
  border-radius: 12px;
  padding: 14px 16px;
  font-size: 14px;
  line-height: 1.5;
}

.login-form__toast {
  border: 1px solid rgba(255, 152, 0, 0.45);
  background: rgba(255, 152, 0, 0.12);
  color: #ff9800;
}

.login-form__submit-error {
  border: 1px solid rgba(255, 107, 107, 0.45);
  background: rgba(255, 107, 107, 0.12);
  color: #ff6b6b;
}

@media (max-width: 640px) {
  .login-form {
    padding: 0 12px 56px;
  }

  .login-form__card {
    padding: 36px 22px 30px;
  }

  .login-form__title {
    font-size: 32px;
  }

  .login-form__options {
    align-items: flex-start;
    flex-direction: column;
    gap: 14px;
  }
}
</style>