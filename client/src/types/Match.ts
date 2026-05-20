export type MatchStatus =
    | 'pending'
    | 'scheduled'
    | 'active'
    | 'completed'
    | 'bye'

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