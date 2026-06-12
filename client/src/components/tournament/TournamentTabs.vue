<script setup lang="ts">
defineProps<{
  activeTab: string
}>()

const emit = defineEmits<{
  (
    e: 'change',
    value: string,
  ): void
}>()
</script>

<template>
  <section class="tabs">
    <button
      class="tab"
      :class="{
        active: activeTab === 'overview',
      }"
      @click="emit('change', 'overview')"
    >
      Overview
    </button>

    <button
      class="tab"
      :class="{
        active:
          activeTab === 'participants',
      }"
      @click="
        emit('change', 'participants')
      "
    >
      Participants
    </button>

    <button
      class="tab"
      :class="{
        active: activeTab === 'grid',
      }"
      @click="emit('change', 'grid')"
    >
      Player grid
    </button>

    <button
      class="tab"
      :class="{
        active: activeTab === 'events',
      }"
      @click="emit('change', 'events')"
    >
      Other events
    </button>

    <div class="line"></div>
  </section>
</template>

<style scoped>
.tabs {
  margin-top: 124px;

  margin-left: 80px;
  margin-right: 80px;
}

.tab {
  width: 105px;
  height: 37px;

  margin-right: 11px;

  border-radius: 12px;
  border: 1px solid #1531ce;

  background: rgba(55, 71, 79, 0.31);

  color: white;
  font-size: 14px;

  cursor: pointer;
}

.active {
  background: #1531ce;
}

.line {
  margin-top: 10px;

  width: 100%;
  height: 1px;

  background: #1531ce;
}
</style>