import axiosInstance from '../api/axiosInstance';
import type { MatchInfo } from '../types/Match';

class MatchService {
    async getTournamentMatches(
        tournamentId: string,
    ): Promise<MatchInfo[]> {
        const response = await axiosInstance.get<MatchInfo[]>(
            `/api/v1/matches/${tournamentId}`,
        );

        return response.data;
    }
}

export const matchService = new MatchService();