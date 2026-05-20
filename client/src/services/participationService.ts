import axiosInstance from '../api/axiosInstance'
import type { Participant } from '../types/Participant'

export const participationService = {
    async getTournamentParticipants(
        tournamentId: string,
    ): Promise<Participant[]> {
        const response = await axiosInstance.get(
            `/tournaments/${tournamentId}/participants`,
        )

        return response.data
    },
}