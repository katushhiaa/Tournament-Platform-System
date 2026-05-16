export type MatchStatus =
    | 'Pending'
    | 'Active'
    | 'Completed';

export interface IBracketPlayer {
    id: string;
    name: string;
}

export interface IMatchInfo {
    id: string;

    player1: IBracketPlayer | null;
    player2: IBracketPlayer | null;

    status: MatchStatus;

    winnerId?: string | null;
}

export interface IBracketRound {
    round: number;
    matches: IMatchInfo[];
}

export type IBracketStructure = BracketRound[]

export interface Match {
  matchId: string
  tournamentId: string
  round: number
  orderNumber: number
  player1Id: string | null
  player2Id: string | null
  status: string
  isBye: boolean
  scorePlayer1: number | null
  scorePlayer2: number | null
  winnerId: string | null
}

export interface BracketRound {
  round: number
  matches: Match[]
  matchesCount: number
  notByeMatchesCount: number
  roundDisplayName: string
}
