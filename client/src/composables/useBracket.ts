import { ref } from 'vue';
import { matchService } from '../services/matchService';
import { buildBracket } from '../utils/buildBracket';
import type { IBracket } from '../utils/buildBracket';

export function useBracket() {
    const bracket = ref<IBracket | null>(null);
    const loading = ref(false);
    const error = ref<string | null>(null);

    const loadBracket = async (tournamentId: string) => {
        loading.value = true;
        error.value = null;

        try {
            const matches =
                await matchService.getTournamentMatches(
                    tournamentId,
                );

            bracket.value = buildBracket(matches);
        } catch (e) {
            error.value = 'Failed to load bracket';
        } finally {
            loading.value = false;
        }
    };

    return {
        bracket,
        loading,
        error,
        loadBracket,
    };
}