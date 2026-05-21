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

    async addParticipant(
        tournamentId: string,
        userId: string,
    ): Promise<void> {
        await axiosInstance.post(
            `/tournaments/${tournamentId}/participants`,
            { userId },
        )
    },

    async removeParticipant(
        tournamentId: string,
        userId: string,
        ): Promise<void> {
        await axiosInstance.delete(
            `/tournaments/${tournamentId}/participants/${userId}`,
        )
    },
}

