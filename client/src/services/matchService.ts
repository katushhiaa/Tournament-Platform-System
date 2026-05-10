import axiosInstance from '../api/axiosInstance';
import type { IMatchDto } from '../types/Match';

class MatchService {
    async getTournamentMatches(
        tournamentId: string,
    ): Promise<IMatchDto[]> {
        const response = await axiosInstance.get<IMatchDto[]>(
            `/api/v1/matches/${tournamentId}`,
        );

        return response.data;
    }
}

export const matchService = new MatchService();