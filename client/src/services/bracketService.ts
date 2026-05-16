import axiosInstance from '../api/axiosInstance'

import type { BracketRound } from '../types/Bracket'

export const bracketService = {
    async getBracket(id: string): Promise<BracketRound[]> {
        const response = await axiosInstance.get(
            `/tournaments/${id}/matches`,
        )

        return response.data
    },
}