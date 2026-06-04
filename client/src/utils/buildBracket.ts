import type { MatchInfo } from '../types/Match';

export interface IBracketMatch extends MatchInfo { }

export interface IBracketRound {
    round: number;
    matches: IBracketMatch[];
}

export interface IBracket {
    rounds: IBracketRound[];
}

export function buildBracket(
    matches: MatchInfo[],
): IBracket {
    const grouped = new Map<number, MatchInfo[]>();
    for (const match of matches) {
        if (!grouped.has(match.round)) {
            grouped.set(match.round, []);
        }
        grouped.get(match.round)!.push(match);
    }
    const rounds: IBracketRound[] = Array.from(
        grouped.entries(),
    )
        .map(([round, matches]) => ({
            round,
            matches: matches.sort(
                (a, b) => a.orderNumber - b.orderNumber,
            ),
        }))
        .sort((a, b) => a.round - b.round);
    return { rounds };
}