<template>
  <div class="modal-overlay">
    <div class="add-players-modal">
      <h2 class="add-players-modal__title">Add players</h2>

      <section class="add-players-modal__section">
        <label class="add-players-modal__label">Copy link</label>

        <div class="add-players-modal__input-wrapper">
          <input :value="joinLink" class="add-players-modal__input add-players-modal__input--link" readonly />

          <button type="button" class="add-players-modal__icon-button" @click="copyLink">
            {{ isCopied ? '✓' : '🔗' }}
          </button>
        </div>

        <p v-if="isCopied" class="add-players-modal__success">Link copied</p>
      </section>

      <div class="add-players-modal__divider">
        <span></span>
        <strong>OR</strong>
        <span></span>
      </div>

      <section class="add-players-modal__section">
        <label class="add-players-modal__label" for="playerSearch">
          Search by user
        </label>

        <div class="add-players-modal__input-wrapper">
          <input
            id="playerSearch"
            v-model.trim="searchQuery"
            class="add-players-modal__input"
            placeholder="Find players by username..."
            autocomplete="off"
          />
          <span class="add-players-modal__search-icon">
            {{ isSearching ? '...' : '⌕' }}
          </span>
        </div>

        <ul v-if="searchResults.length && !selectedUser" class="add-players-modal__dropdown">
          <li
            v-for="user in searchResults"
            :key="user.id"
            class="add-players-modal__dropdown-item"
            @click="selectUser(user)"
          >
            {{ user.fullName }}
          </li>
        </ul>

        <p v-if="addError" class="add-players-modal__error">{{ addError }}</p>
        <p v-if="addSuccess" class="add-players-modal__success">Player added successfully!</p>
      </section>

      <div class="add-players-modal__actions">
        <button
          type="button"
          class="add-players-modal__button add-players-modal__button--cancel"
          @click="handleCancel"
        >
          Cancel
        </button>

        <button
          type="button"
          class="add-players-modal__button add-players-modal__button--add"
          :disabled="isAdding || !selectedUser"
          @click="handleAdd"
        >
          {{ isAdding ? 'Adding...' : 'Add' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { userService, type IUserSearchItem } from '../../services/userService'
import { participationService } from '../../services/participationService'

const props = defineProps<{
  tournamentId: string
}>()

const emit = defineEmits<{
  close: []
  playerAdded: []
}>()

const searchQuery = ref('')
const isCopied = ref(false)
const searchResults = ref<IUserSearchItem[]>([])
const selectedUser = ref<IUserSearchItem | null>(null)
const isSearching = ref(false)
const isAdding = ref(false)
const addError = ref<string | null>(null)
const addSuccess = ref(false)

const joinLink = computed(() =>
  `${window.location.origin}/join/${props.tournamentId}`
)

const copyLink = async () => {
  await navigator.clipboard.writeText(joinLink.value)
  isCopied.value = true
  window.setTimeout(() => { isCopied.value = false }, 1800)
}

let searchTimeout: ReturnType<typeof setTimeout>
let isSelecting = false

watch(searchQuery, (val) => {
  if (isSelecting) return

  selectedUser.value = null
  addError.value = null
  addSuccess.value = false
  clearTimeout(searchTimeout)

  if (!val) {
    searchResults.value = []
    return
  }

  searchTimeout = setTimeout(async () => {
    try {
      isSearching.value = true
      const results = await userService.searchUsers(val)
      searchResults.value = results.filter(u =>
        u.fullName.toLowerCase().includes(val.toLowerCase())
      )
    } catch {
      searchResults.value = []
    } finally {
      isSearching.value = false
    }
  }, 350)
})

const selectUser = (user: IUserSearchItem) => {
  isSelecting = true
  selectedUser.value = user
  searchQuery.value = user.fullName
  searchResults.value = []
  addError.value = null
  addSuccess.value = false
  setTimeout(() => { isSelecting = false }, 0)
}

const handleAdd = async () => {
  if (!selectedUser.value) {
    addError.value = 'Please select a user from the list.'
    return
  }

  try {
    isAdding.value = true
    addError.value = null
    await participationService.addParticipant(
      props.tournamentId,
      selectedUser.value.id,
    )
    addSuccess.value = true
    searchQuery.value = ''
    selectedUser.value = null
    searchResults.value = []
    emit('playerAdded')
  } catch (e: any) {
    if (e?.response?.status === 409) {
      addError.value = 'This player is already added or the tournament is full.'
    } else {
      addError.value = 'Failed to add player. Please try again.'
    }
  } finally {
    isAdding.value = false
  }
}

const handleCancel = () => {
  emit('close')
}
</script>

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

.add-players-modal {
  width: 100%;
  max-width: 760px;
  border: 1px solid #1531ce;
  border-radius: 28px;
  background: #252e35;
  color: #fffcf2;
  padding: 44px 72px 40px;
}

.add-players-modal__title {
  margin: 0 0 52px;
  text-align: center;
  font-size: 48px;
  font-weight: 800;
}

.add-players-modal__label {
  display: block;
  margin-bottom: 18px;
  font-size: 32px;
}

.add-players-modal__section {
  margin-bottom: 42px;
}

.add-players-modal__input-wrapper {
  position: relative;
}

.add-players-modal__input {
  width: 100%;
  height: 76px;
  border: 2px solid #fffcf2;
  border-radius: 16px;
  background: transparent;
  color: #fffcf2;
  padding: 0 78px 0 24px;
  font-size: 24px;
  outline: none;
  box-sizing: border-box;
}

.add-players-modal__input--link {
  color: #1531ce;
  font-weight: 700;
}

.add-players-modal__input::placeholder {
  color: rgba(255, 252, 242, 0.55);
}

.add-players-modal__icon-button,
.add-players-modal__search-icon {
  position: absolute;
  top: 50%;
  right: 28px;
  transform: translateY(-50%);
  color: #fffcf2;
  font-size: 30px;
}

.add-players-modal__icon-button {
  border: none;
  background: transparent;
  cursor: pointer;
}

.add-players-modal__success {
  margin: 10px 0 0;
  color: #84c082;
}

.add-players-modal__divider {
  display: flex;
  align-items: center;
  gap: 28px;
  margin: 28px 70px 42px;
}

.add-players-modal__divider span {
  flex: 1;
  height: 2px;
  background: #fffcf2;
}

.add-players-modal__divider strong {
  font-size: 24px;
}

.add-players-modal__actions {
  display: flex;
  justify-content: flex-end;
  gap: 32px;
}

.add-players-modal__button {
  width: 180px;
  height: 60px;
  border-radius: 16px;
  font-size: 24px;
  font-weight: 800;
  cursor: pointer;
}

.add-players-modal__button--cancel {
  border: 1px solid #1531ce;
  background: transparent;
  color: #1531ce;
}

.add-players-modal__button--add {
  border: 1px solid #ff9800;
  background: #ff9800;
  color: #fffcf2;
}

.add-players-modal__button--add:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.add-players-modal__dropdown {
  list-style: none;
  margin: 8px 0 0;
  padding: 0;
  border: 1px solid #1531ce;
  border-radius: 12px;
  background: #2e3a42;
  overflow: hidden;
  max-height: 220px;
  overflow-y: auto;
}

.add-players-modal__dropdown-item {
  padding: 14px 24px;
  font-size: 18px;
  cursor: pointer;
  color: #fffcf2;
  transition: background 0.15s ease;
}

.add-players-modal__dropdown-item:hover {
  background: rgba(21, 49, 206, 0.35);
}

.add-players-modal__error {
  margin: 10px 0 0;
  color: #ce0f0f;
  font-size: 14px;
}
</style>