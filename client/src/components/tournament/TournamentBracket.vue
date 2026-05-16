<script setup lang="ts">
/*import type {
  IBracketStructure,
} from '../../types/Bracket'

defineProps<{
  rounds: IBracketStructure
}>()*/
const rounds = [
  [
    'Shevchenko Taras\nHryhorovych',
    'Kovalenko Oksana\nViktorovych',
  ],
  [
    'Melnyk Maria\nIvanivna',
    'Kozak Volodymyr\nPetrovych',
  ],
]
</script>

<template>
  <section class="bracket">
    <h2 class="title">
      Player grid
    </h2>

    <div class="tournament">
      <!-- ROUND 1 -->
      <div class="round first">
        <div
          v-for="(match, index) in rounds" 
          :key="index"
          class="match"
        >
          <div class="player">
            {{ match[0] }}
          </div>

          <div class="player">
            {{ match[1] }}
          </div>
        </div>
      </div>

      <!-- CONNECTORS -->
      <div class="connectors">
        <div class="connector top"></div>
        <div class="connector bottom"></div>
      </div>

      <!-- SEMI -->
      <div class="round second">
        <div class="winner"></div>

        <div class="winner second-winner"></div>
      </div>

      <!-- FINAL CONNECTOR -->
      <div class="final-connector"></div>

      <!-- FINAL -->
      <div class="champion"></div>
    </div>
  </section>
</template>

<style scoped>
.bracket {
  padding: 90px 80px 140px;
}

.title {
  text-align: center;

  font-size: 32px;
  font-weight: 600;

  color: white;

  margin-bottom: 110px;
}

.tournament {
  display: flex;
  align-items: center;
  justify-content: center;
}

/* ROUND 1 */

.round.first {
  display: flex;
  flex-direction: column;
  gap: 120px;
}

.match {
  display: flex;
  flex-direction: column;
  gap: 70px;
}

.player {
  width: 240px;
  min-height: 62px;

  background: #1531ce;

  border-radius: 10px;

  padding: 14px 18px;

  display: flex;
  align-items: center;

}


/* CONNECTORS */

.connectors {
  width: 120px;

  display: flex;
  flex-direction: column;

  gap: 188px;

  margin-left: 0;
}

.connector {
  width: 120px;
  height: 132px;

  border-top: 2px solid rgba(255,255,255,0.7);
  border-right: 2px solid rgba(255,255,255,0.7);
  border-bottom: 2px solid rgba(255,255,255,0.7);
}

/* ROUND 2 */

.round.second {
  display: flex;
  flex-direction: column;

  gap: 170px;

  margin-left: 0;
}

.winner {
  width: 240px;
  height: 62px;

  background: #8e8e8e;

  border-radius: 10px;

  position: relative;
}



/* FINAL CONNECTOR */

.final-connector {
  width: 80px;
  height: 196px;

  border-top: 2px solid rgba(255,255,255,0.7);
  border-right: 2px solid rgba(255,255,255,0.7);
  border-bottom: 2px solid rgba(255,255,255,0.7);
}

/* CHAMPION */

.champion {
  width: 240px;
  height: 62px;

  background: #8e8e8e;

  border-radius: 10px;

  position: relative;
}


@media (max-width: 1200px) {
  .tournament {
    transform: scale(0.75);
    transform-origin: top center;
  }

  .bracket {
    overflow-x: auto;
  }
}
</style>