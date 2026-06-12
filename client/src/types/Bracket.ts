import type { MatchInfo } from './Match'

export interface BracketRound {
    round: number

    matches: MatchInfo[]

    matchesCount: number

    notByeMatchesCount: number

    roundDisplayName: string
}

export type IBracketStructure = BracketRound[]