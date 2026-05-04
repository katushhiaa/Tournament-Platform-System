import axios from 'axios';
import axiosInstance from '../api/axiosInstance';
import type { IApiError } from '../types/Auth';

export interface IThemeOption {
    id: string;
    name: string;
}

export interface ICreateTournamentRequest {
    title: string;
    description: string | null;
    conditions: string | null;
    startDate: string;
    endDate: string;
    registrationCloseDate: string;
    sport: string;
    maxParticipants: number;
}

export interface ITournamentResponse extends ICreateTournamentRequest {
    id: string;
    status: string;
    organizerId: string;
}

type BackendErrorResponse = {
    error?: {
        type?: string;
        message?: string;
    };
};

const toApiError = (error: unknown): IApiError => {
    if (axios.isAxiosError<BackendErrorResponse>(error)) {
        const status = error.response?.status;

        return {
            errorCode:
                status === 400
                    ? 'VALIDATION_ERROR'
                    : status === 401
                        ? 'UNAUTHORIZED'
                        : status === 409
                            ? 'CONFLICT'
                            : 'INTERNAL_ERROR',
            message: error.response?.data?.error?.message ?? 'Server error. Please try again later.',
        };
    }

    return {
        errorCode: 'INTERNAL_ERROR',
        message: 'Server error. Please try again later.',
    };
};

class TournamentService {
    async getThemes(): Promise<IThemeOption[]> {
        try {
            const response = await axiosInstance.get<IThemeOption[]>('/themes');
            return response.data;
        } catch {
            return [
                { id: 'a1b2c3d4-e5f6-7a8b-9c0d-111213141516', name: 'Game' },
                { id: '4b327d50-8852-43da-8fed-d7522ce760aa', name: 'Chess' },
                { id: '710862d5-12cc-48a4-9d3c-3575226524f7', name: 'Tennis' },
            ];
        }
    }

    async createTournament(data: ICreateTournamentRequest): Promise<ITournamentResponse> {
        try {
            const response = await axiosInstance.post<ITournamentResponse>('/tournaments', data);
            return response.data;
        } catch (error) {
            throw toApiError(error);
        }
    }
}

export const tournamentService = new TournamentService();