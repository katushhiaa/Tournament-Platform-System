export type MatchStatus =
    | 'pending'
    | 'active'
    | 'completed';

export interface IMatchDto {
    matchId: string;
    tournamentId: string;
    round: number;
    orderNumber: number;

    player1Id: string;
    player2Id: string;

    status: MatchStatus;

    scorePlayer1?: number;
    scorePlayer2?: number;

    winnerId?: string;
}