import axios from 'axios';
import axiosInstance from '../api/axiosInstance';
import type { IApiError } from '../types/Auth';
import type { IThemeOption, ITournament, ITournamentCreate, ITournamentResponse } from '../types/Tournament';

type BackendErrorResponse = {
    error?: {
        code?: number;
        type?: string;
        message?: string;
        path?: string;
        timestamp?: string;
        traceId?: string;
    };
};

const buildTournamentApiError = (error: unknown): IApiError => {
    if (axios.isAxiosError<BackendErrorResponse>(error)) {
        const status = error.response?.status;
        const message = error.response?.data?.error?.message;

        if (status === 400) {
            return { errorCode: 'VALIDATION_ERROR', message: message ?? 'Invalid tournament data.' };
        }

        if (status === 401) {
            return { errorCode: 'UNAUTHORIZED', message: message ?? 'You must be authorized.' };
        }

        if (status === 403) {
            return { errorCode: 'ACCESS_FORBIDDEN', message: message ?? 'Access forbidden.' };
        }

        if (status === 409) {
            return { errorCode: 'CONFLICT', message: message ?? 'Tournament conflict.' };
        }

        if (status && status >= 500) {
            return { errorCode: 'INTERNAL_ERROR', message: message ?? 'Server error. Please try again later.' };
        }
    }

    return {
        errorCode: 'INTERNAL_ERROR',
        message: 'Server error. Please try again later.',
    };
};

const buildRegistrationUrl = (tournamentId: string): string => {
    return `${window.location.origin}/join/${tournamentId}`;
};

class TournamentService {
    async createTournament(data: ITournamentCreate): Promise<ITournamentResponse> {
        try {
            const response = await axiosInstance.post<ITournament>('/tournaments', data);

            return {
                ...response.data,
                registrationUrl: buildRegistrationUrl(response.data.id),
            };
        } catch (error) {
            throw buildTournamentApiError(error);
        }
    }

    async getTournamentById(id: string): Promise<ITournament> {
        try {
            const response = await axiosInstance.get<ITournament>(`/tournaments/${id}`);
            return response.data;
        } catch (error) {
            throw buildTournamentApiError(error);
        }
    }

    async startTournament(id: string): Promise<string> {
        try {
            const response = await axiosInstance.post<string>(`/tournaments/${id}/start`);
            return response.data;
        } catch (error) {
            throw buildTournamentApiError(error);
        }
    }

    async getThemes(): Promise<IThemeOption[]> {
        return [
            { id: 'a1b2c3d4-e5f6-7a8b-9c0d-111213141516', name: 'Game' },
            { id: 'c9033c3a-f011-4491-9b69-c834e0ec968e', name: 'Sport' },
            { id: '7a04751a-36c1-48cd-a867-ec9e57589050', name: 'Other' },
        ];
    }
}

export const tournamentService = new TournamentService();