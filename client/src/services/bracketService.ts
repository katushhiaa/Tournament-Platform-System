import axios from 'axios'

import type {
    IBracketStructure,
} from '../types/Bracket'

export const bracketService = {
    async getBracket(
        tournamentId: string,
    ): Promise<IBracketStructure> {
        try {
            const response = await axios.get(
                `/api/v1/tournaments/${tournamentId}/matches`,
            )

            return response.data
        } catch (error: any) {
            console.error(
                'Failed to load bracket:',
                error,
            )

            /*
              Якщо сітка ще не створена —
              повертаємо порожній масив.
            */

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