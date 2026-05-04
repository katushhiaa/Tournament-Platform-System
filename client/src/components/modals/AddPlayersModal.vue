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
        <label class="add-players-modal__label" for="playerSearch">Search by user</label>

        <div class="add-players-modal__input-wrapper">
          <input
            id="playerSearch"
            v-model.trim="searchQuery"
            class="add-players-modal__input"
            placeholder="Find players by username..."
          />
          <span class="add-players-modal__search-icon">⌕</span>
        </div>
      </section>

      <div class="add-players-modal__actions">
        <button type="button" class="add-players-modal__button add-players-modal__button--cancel" @click="handleCancel">
          Cancel
        </button>

        <button type="button" class="add-players-modal__button add-players-modal__button--add" @click="handleAdd">
          Add
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';

const props = defineProps<{
  tournamentId: string;
}>();

const router = useRouter();
const searchQuery = ref('');
const isCopied = ref(false);

const joinLink = computed(() => `${window.location.origin}/join/${props.tournamentId}`);

const copyLink = async () => {
  await navigator.clipboard.writeText(joinLink.value);
  isCopied.value = true;

  window.setTimeout(() => {
    isCopied.value = false;
  }, 1800);
};

const handleAdd = () => {
  console.log('[AddPlayersModal] add player:', searchQuery.value);
};

const handleCancel = () => {
  router.push('/my-tournaments');
};
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
</style>