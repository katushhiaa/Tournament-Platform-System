export type MatchStatus =
    | 'pending'
    | 'scheduled'
    | 'active'
    | 'completed'
    | 'bye'

export interface MatchPlayer {
    id: string | null
}

export interface MatchInfo {
    matchId: string
    tournamentId: string

    round: number
    orderNumber: number

    player1Id: string | null
    player2Id: string | null

    status: MatchStatus

    isBye: boolean

    scorePlayer1: number | null
    scorePlayer2: number | null

    winnerId: string | null
}

export interface BracketRound {
    round: number

    matches: MatchInfo[]

    matchesCount: number

    notByeMatchesCount: number

    roundDisplayName: string
}

export type IBracketStructure = BracketRound[]