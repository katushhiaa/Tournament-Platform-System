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

export interface IBracketStructure {
    rounds: IBracketRound[];
}