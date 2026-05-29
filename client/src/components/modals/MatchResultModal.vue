<script setup lang="ts">
import { computed, ref } from 'vue'
import { bracketService } from '../../services/bracketService'
import type { MatchInfo } from '../../types/Match'

const props = defineProps<{
  tournamentId: string
  match: MatchInfo
}>()

const emit = defineEmits<{
  close: []
  saved: []
}>()

const score1 = ref<string>('')
const score2 = ref<string>('')
const error = ref<string | null>(null)
const isSaving = ref(false)

const player1Name = computed(() => props.match.player1Name ?? 'Player 1')
const player2Name = computed(() => props.match.player2Name ?? 'Player 2')

const validate = (): boolean => {
  error.value = null

  if (score1.value === '' || score2.value === '') {
    error.value = 'Будь ласка, введіть рахунок матчу'
    return false
  }

  const s1 = Number(score1.value)
  const s2 = Number(score2.value)

  if (!Number.isInteger(s1) || !Number.isInteger(s2) || isNaN(s1) || isNaN(s2)) {
    error.value = 'Рахунок повинен бути цілим числом'
    return false
  }

  if (s1 < 0 || s2 < 0) {
    error.value = 'Рахунок не може бути від\'ємним'
    return false
  }

  if (s1 === s2) {
    error.value = 'Нічия неможлива у матчах на виліт. Визначте переможця'
    return false
  }

  return true
}

const handleSave = async () => {
  if (!validate()) return

  const s1 = Number(score1.value)
  const s2 = Number(score2.value)
  const winnerId = s1 > s2 ? props.match.player1Id! : props.match.player2Id!

  try {
    isSaving.value = true
    error.value = null
    await bracketService.saveMatchResult(props.tournamentId, props.match.matchId, {
      scorePlayer1: s1,
      scorePlayer2: s2,
      winnerId,
    })
    emit('saved')
  } catch (e: any) {
    const msg = e?.response?.data?.error?.message
    if (e?.response?.status === 409) {
      error.value = 'Результат для цього матчу вже збережено'
    } else if (e?.response?.status === 400) {
      error.value = msg ?? 'Некоректний результат матчу'
    } else {
      error.value = msg ?? 'Не вдалося зберегти результат. Спробуйте ще раз'
    }
  } finally {
    isSaving.value = false
  }
}
</script>

<template>
  <div class="modal-overlay" @mousedown.self="emit('close')">
    <div class="modal">
      <h2 class="modal__title">MATCH RESULT</h2>

      <!-- Player 1 -->
      <div class="modal__field">
        <label class="modal__label">Player 1</label>
        <div class="modal__input-wrapper modal__input-wrapper--readonly">
          <svg class="modal__icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="12" cy="8" r="4" stroke="rgba(255,255,255,0.5)" stroke-width="1.5"/>
            <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" stroke="rgba(255,255,255,0.5)" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
          <span class="modal__player-name">{{ player1Name }}</span>
        </div>
      </div>

      <!-- Score Player 1 -->
      <div class="modal__field">
        <label class="modal__label">Score Player 1</label>
        <input
          v-model="score1"
          class="modal__input"
          type="number"
          min="0"
          placeholder="Enter the score"
          :disabled="isSaving"
          @input="error = null"
        />
        <p class="modal__hint">Example text: "e.g. 1"</p>
      </div>

      <!-- Player 2 -->
      <div class="modal__field">
        <label class="modal__label">Player 2</label>
        <div class="modal__input-wrapper modal__input-wrapper--readonly">
          <svg class="modal__icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <circle cx="12" cy="8" r="4" stroke="rgba(255,255,255,0.5)" stroke-width="1.5"/>
            <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" stroke="rgba(255,255,255,0.5)" stroke-width="1.5" stroke-linecap="round"/>
          </svg>
          <span class="modal__player-name">{{ player2Name }}</span>
        </div>
      </div>

      <!-- Score Player 2 -->
      <div class="modal__field">
        <label class="modal__label">Score Player 2</label>
        <input
          v-model="score2"
          class="modal__input"
          type="number"
          min="0"
          placeholder="Enter the score"
          :disabled="isSaving"
          @input="error = null"
        />
        <p class="modal__hint">Example text: "e.g. 2"</p>
      </div>

      <!-- Actions -->
      <div class="modal__actions">
        <button
          class="modal__btn modal__btn--cancel"
          :disabled="isSaving"
          @click="emit('close')"
        >Cancel</button>
        <button
          class="modal__btn modal__btn--save"
          :disabled="isSaving"
          @click="handleSave"
        >{{ isSaving ? 'Saving...' : 'Save' }}</button>
      </div>

      <!-- Error -->
      <div v-if="error" class="modal__error-box">
        <svg width="20" height="20" viewBox="0 0 20 20" fill="none">
          <circle cx="10" cy="10" r="9" stroke="#e57373" stroke-width="1.5"/>
          <path d="M10 6v5M10 13v1" stroke="#e57373" stroke-width="1.5" stroke-linecap="round"/>
        </svg>
        <span>{{ error }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.65);
  padding: 24px;
}

.modal {
  width: 100%;
  max-width: 600px;
  border: 1px solid #1531ce;
  border-radius: 28px;
  background: #252e35;
  color: #fffcf2;
  padding: 44px 56px 40px;
  display: flex;
  flex-direction: column;
  gap: 0;
}

.modal__title {
  margin: 0 0 36px;
  text-align: center;
  font-size: 42px;
  font-weight: 800;
  letter-spacing: 0.04em;
}

.modal__field {
  margin-bottom: 24px;
}

.modal__label {
  display: block;
  font-size: 22px;
  font-weight: 700;
  margin-bottom: 10px;
}

.modal__input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.modal__input-wrapper--readonly {
  height: 56px;
  border-radius: 14px;
  background: rgba(21, 49, 206, 0.35);
  border: 1px solid #1531ce;
  padding: 0 20px;
  gap: 14px;
}

.modal__icon {
  width: 22px;
  height: 22px;
  flex-shrink: 0;
}

.modal__player-name {
  font-size: 16px;
  color: rgba(255, 255, 255, 0.7);
}

.modal__input {
  width: 100%;
  height: 56px;
  border: 1px solid #1531ce;
  border-radius: 14px;
  background: rgba(21, 49, 206, 0.35);
  color: #fffcf2;
  padding: 0 20px;
  font-size: 16px;
  outline: none;
  transition: border-color 0.2s;
}

.modal__input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.modal__input:focus {
  border-color: rgba(99, 130, 255, 0.8);
}

.modal__input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* hide number input arrows */
.modal__input[type=number]::-webkit-inner-spin-button,
.modal__input[type=number]::-webkit-outer-spin-button {
  -webkit-appearance: none;
}
.modal__input[type=number] { -moz-appearance: textfield; appearance: textfield; }

.modal__hint {
  margin: 6px 0 0;
  font-size: 13px;
  color: rgba(255, 255, 255, 0.45);
}

.modal__actions {
  display: flex;
  justify-content: flex-end;
  gap: 20px;
  margin-top: 8px;
  margin-bottom: 20px;
}

.modal__btn {
  width: 148px;
  height: 52px;
  border-radius: 14px;
  font-size: 18px;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.2s;
}

.modal__btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.modal__btn--cancel {
  border: 1px solid #1531ce;
  background: transparent;
  color: #1531ce;
}

.modal__btn--save {
  border: none;
  background: #ff9800;
  color: #fff;
}

.modal__btn--save:hover:not(:disabled) {
  opacity: 0.88;
}

.modal__error-box {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 20px;
  border: 1px solid #e57373;
  border-radius: 14px;
  color: #e57373;
  font-size: 14px;
}
</style>