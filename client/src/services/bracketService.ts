import axiosInstance from '../api/axiosInstance'
import type { IBracketStructure } from '../types/Bracket'

export const bracketService = {
    async getBracket(
        tournamentId: string,
    ): Promise<IBracketStructure> {
        try {
            const response = await axiosInstance.get(
                `/tournaments/${tournamentId}/matches`,
            )

            return response.data
        } catch (error: any) {
            if (
                error.response?.status === 404 ||
                error.response?.status === 400
            ) {
                return []
            }

            throw error
        }
    },
}