<script setup lang="ts">
import { computed, ref } from 'vue'
import type { IBracketStructure } from '../../types/Bracket'
import type { MatchInfo } from '../../types/Match'
import MatchResultModal from '../modals/MatchResultModal.vue'

const props = defineProps<{
  rounds: IBracketStructure
  isOrganizer?: boolean
  tournamentId: string
  isLoading?: boolean
}>()

const emit = defineEmits<{ bracketUpdated: [] }>()

const selectedMatch = ref<MatchInfo | null>(null)

const canClick = (match: MatchInfo) =>
  props.isOrganizer &&
  !match.isBye &&
  !!match.player1Id &&
  !!match.player2Id &&
  match.status !== 'completed'

const handleMatchClick = (match: MatchInfo) => {
  if (canClick(match)) selectedMatch.value = match
}

const handleSaved = () => {
  selectedMatch.value = null
  emit('bracketUpdated')
}

const CARD_H    = 40
const CARD_GAP  = 4
const MATCH_H   = CARD_H * 2 + CARD_GAP
const MATCH_GAP = 32
const LABEL_H   = 56
const TOP_PAD   = 24
const ROUND_W   = 228
const CONN_W    = 64

const C_ACTIVE   = '#2563eb'
const C_ACTIVE_S = 'rgba(77,110,255,0.45)'
const C_EMPTY    = 'rgba(255,255,255,0.06)'
const C_EMPTY_S  = 'rgba(255,255,255,0.18)'
const C_WIN      = '#166534'
const C_WIN_S    = 'rgba(52,211,100,0.45)'
const C_ELIM     = 'rgba(255,255,255,0.08)'
const C_ELIM_S   = 'rgba(255,255,255,0.1)'
const C_TEXT     = '#ffffff'
const C_MUTED    = 'rgba(255,255,255,0.22)'
const C_CONN     = 'rgba(255,255,255,0.3)'
const C_LABEL    = 'rgba(255,255,255,0.42)'

const getMatchCentersY = (roundIndex: number): number[] => {
  const matchCount = props.rounds[roundIndex].matches.length
  if (roundIndex === 0) {
    return Array.from({ length: matchCount }, (_, i) =>
      LABEL_H + TOP_PAD + i * (MATCH_H + MATCH_GAP) + MATCH_H / 2
    )
  }
  const prevCenters = getMatchCentersY(roundIndex - 1)
  return Array.from({ length: matchCount }, (_, i) => {
    const a = prevCenters[i * 2] ?? prevCenters[prevCenters.length - 1]
    const b = prevCenters[i * 2 + 1] ?? a
    return (a + b) / 2
  })
}

const svgH = computed(() => {
  const firstRound = props.rounds[0]
  if (!firstRound) return 200
  const n = firstRound.matches.length
  return LABEL_H + TOP_PAD + n * MATCH_H + (n - 1) * MATCH_GAP + TOP_PAD
})

const svgW = computed(() =>
  props.rounds.length * ROUND_W + (props.rounds.length - 1) * CONN_W
)

const getRoundLabel = (name: string) => {
  const map: Record<string, string> = {
    quarterfinal: 'QUARTERFINAL',
    semifinal: 'SEMIFINAL',
    final: 'FINAL',
  }
  return map[name.toLowerCase()] ?? name.toUpperCase()
}

const getPlayerName = (name: string | null, id: string | null) => {
  if (name) return name
  if (id) return id.slice(0, 8) + '...'
  return 'TBD'
}

const cardFill = (match: MatchInfo, player: 1 | 2) => {
  const pid = player === 1 ? match.player1Id : match.player2Id
  if (!pid) return C_EMPTY
  if (!match.isBye && match.winnerId && match.winnerId === pid) return C_WIN
  if (!match.isBye && match.winnerId && match.winnerId !== pid) return C_ELIM
  return C_ACTIVE
}

const cardStroke = (match: MatchInfo, player: 1 | 2) => {
  const pid = player === 1 ? match.player1Id : match.player2Id
  if (!pid) return C_EMPTY_S
  if (!match.isBye && match.winnerId && match.winnerId === pid) return C_WIN_S
  if (!match.isBye && match.winnerId && match.winnerId !== pid) return C_ELIM_S
  return C_ACTIVE_S
}

const cardTextFill = (match: MatchInfo, player: 1 | 2) => {
  const pid = player === 1 ? match.player1Id : match.player2Id
  if (!pid) return C_MUTED
  if (!match.isBye && match.winnerId && match.winnerId !== pid) return 'rgba(255,255,255,0.35)'
  return C_TEXT
}

interface RenderMatch {
  match: MatchInfo
  x: number
  cy: number
  y1: number
  y2: number
}

const renderData = computed(() => {
  if (!props.rounds.length) return { rounds: [], connectors: [] }

  type ConnLine = { x1: number; y1: number; x2: number; y2: number }
  const allConnLines: ConnLine[] = []

  const rounds = props.rounds.map((round, rIdx) => {
    const x = rIdx * (ROUND_W + CONN_W)
    const centers = getMatchCentersY(rIdx)

    const matches: RenderMatch[] = round.matches.map((match, mIdx) => {
      const cy = centers[mIdx]
      return {
        match,
        x,
        cy,
        y1: cy - MATCH_H / 2,
        y2: cy - MATCH_H / 2 + CARD_H + CARD_GAP,
      }
    })

    if (rIdx < props.rounds.length - 1) {
      const nextCenters = getMatchCentersY(rIdx + 1)
      const nextCount = props.rounds[rIdx + 1].matches.length
      const x1 = x + ROUND_W
      const xMid = x1 + CONN_W / 2
      const x2 = x1 + CONN_W

      for (let ni = 0; ni < nextCount; ni++) {
        const src1 = centers[ni * 2] ?? centers[centers.length - 1]
        const src2 = centers[ni * 2 + 1] ?? src1
        const toY  = nextCenters[ni]

        allConnLines.push({ x1, y1: src1, x2: xMid, y2: src1 })
        if (src2 !== src1) {
          allConnLines.push({ x1, y1: src2, x2: xMid, y2: src2 })
          allConnLines.push({ x1: xMid, y1: src1, x2: xMid, y2: src2 })
        }
        allConnLines.push({ x1: xMid, y1: toY, x2, y2: toY })
      }
    }

    return {
      label: getRoundLabel(round.roundDisplayName),
      labelX: x + ROUND_W / 2,
      matches,
    }
  })

  return { rounds, connectors: allConnLines }
})
</script>

<template>
  <section class="bracket">
    <h2 class="bracket__title">Player grid</h2>

    <div v-if="!rounds.length" class="bracket__empty">
      Bracket has not been generated yet
    </div>

    <div v-else class="bracket__scroll">
      <div v-if="isLoading" class="bracket__loading">
        Updating bracket...
      </div>
      <svg
        :width="svgW"
        :height="svgH"
        :viewBox="`0 0 ${svgW} ${svgH}`"
        xmlns="http://www.w3.org/2000/svg"
        overflow="visible"
      >
       
        <text
          v-for="r in renderData.rounds"
          :key="'lbl-' + r.label"
          :x="r.labelX"
          :y="LABEL_H / 2"
          text-anchor="middle"
          dominant-baseline="central"
          :fill="C_LABEL"
          font-size="11"
          font-weight="700"
          letter-spacing="1.4"
          font-family="inherit"
        >{{ r.label }}</text>

       
        <line
          v-for="(l, i) in renderData.connectors"
          :key="'conn-' + i"
          :x1="l.x1" :y1="l.y1"
          :x2="l.x2" :y2="l.y2"
          :stroke="C_CONN"
          stroke-width="1.5"
          stroke-linecap="round"
        />

      
        <g
          v-for="r in renderData.rounds"
          :key="'round-' + r.label"
        >
          <g
            v-for="rm in r.matches"
            :key="rm.match.matchId"
            :class="{ 'match--clickable': canClick(rm.match) }"
            @click="handleMatchClick(rm.match)"
          >
           
            <text
              v-if="rm.match.isBye"
              :x="rm.x + 6"
              :y="rm.y1 - 7"
              :fill="C_MUTED"
              font-size="9"
              font-weight="700"
              letter-spacing="1"
              font-family="inherit"
            >BYE</text>

           
            <rect
              :x="rm.x" :y="rm.y1"
              :width="ROUND_W" :height="CARD_H"
              rx="7"
              :fill="cardFill(rm.match, 1)"
              :stroke="cardStroke(rm.match, 1)"
              stroke-width="1"
            />
            <text
              :x="rm.x + 14"
              :y="rm.y1 + CARD_H / 2"
              dominant-baseline="central"
              :fill="cardTextFill(rm.match, 1)"
              font-size="13"
              font-weight="500"
              font-family="inherit"
            >{{ getPlayerName(rm.match.player1Name, rm.match.player1Id) }}</text>
            <text
              v-if="rm.match.status === 'completed'"
              :x="rm.x + ROUND_W - 14"
              :y="rm.y1 + CARD_H / 2"
              dominant-baseline="central"
              text-anchor="end"
              :fill="cardTextFill(rm.match, 1)"
              font-size="14"
              font-weight="700"
              font-family="inherit"
            >{{ rm.match.scorePlayer1 }}</text>

           
            <rect
              :x="rm.x" :y="rm.y2"
              :width="ROUND_W" :height="CARD_H"
              rx="7"
              :fill="rm.match.isBye ? C_EMPTY : cardFill(rm.match, 2)"
              :stroke="rm.match.isBye ? C_EMPTY_S : cardStroke(rm.match, 2)"
              stroke-width="1"
              :stroke-dasharray="rm.match.isBye ? '5 3' : undefined"
            />
            <text
              :x="rm.x + 14"
              :y="rm.y2 + CARD_H / 2"
              dominant-baseline="central"
              :fill="rm.match.isBye ? C_MUTED : cardTextFill(rm.match, 2)"
              font-size="13"
              font-weight="500"
              font-family="inherit"
            >{{ rm.match.isBye ? '—' : getPlayerName(rm.match.player2Name, rm.match.player2Id) }}</text>
            <text
              v-if="rm.match.status === 'completed'"
              :x="rm.x + ROUND_W - 14"
              :y="rm.y2 + CARD_H / 2"
              dominant-baseline="central"
              text-anchor="end"
              :fill="cardTextFill(rm.match, 2)"
              font-size="14"
              font-weight="700"
              font-family="inherit"
            >{{ rm.match.scorePlayer2 }}</text>
          </g>
        </g>
      </svg>
    </div>

    
    <MatchResultModal
      v-if="selectedMatch"
      :tournament-id="tournamentId"
      :match="selectedMatch"
      @close="selectedMatch = null"
      @saved="handleSaved"
    />
  </section>
</template>

<style scoped>
.bracket {
  padding: 60px 80px 140px;
}

.bracket__title {
  text-align: center;
  font-size: 32px;
  font-weight: 700;
  color: white;
  margin-bottom: 48px;
}

.bracket__empty {
  text-align: center;
  color: rgba(255, 255, 255, 0.5);
  font-size: 16px;
}

.bracket__scroll {
  overflow-x: auto;
  padding-bottom: 16px;
}

.match--clickable {
  cursor: pointer;
}

.match--clickable:hover rect {
  filter: brightness(1.12);
}
.bracket__loading {
  text-align: center;
  color: rgba(255, 255, 255, 0.6);
  font-size: 15px;
  padding: 20px 0;
  animation: pulse 1.2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

@media (max-width: 900px) {
  .bracket {
    padding: 40px 16px 100px;
  }
}
</style>