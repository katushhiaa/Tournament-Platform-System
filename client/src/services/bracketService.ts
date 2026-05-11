import axios from 'axios';
import axiosInstance from '../api/axiosInstance';

import type { IApiError } from '../types/Auth';
import type { IBracketStructure } from '../types/Bracket';

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

const buildBracketApiError = (error: unknown): IApiError => {
    if (axios.isAxiosError<BackendErrorResponse>(error)) {
        const status = error.response?.status;
        const message = error.response?.data?.error?.message;

        if (status === 401) {
            return {
                errorCode: 'UNAUTHORIZED',
                message: message ?? 'You must be authorized.',
            };
        }

        if (status === 403) {
            return {
                errorCode: 'ACCESS_FORBIDDEN',
                message: message ?? 'Access forbidden.',
            };
        }

        if (status === 404) {
            return {
                errorCode: 'NOT_FOUND',
                message: message ?? 'Tournament not found.',
            };
        }

        if (status === 409) {
            return {
                errorCode: 'CONFLICT',
                message:
                    message ??
                    'Bracket is not generated yet.',
            };
        }

        if (status && status >= 500) {
            return {
                errorCode: 'INTERNAL_ERROR',
                message:
                    message ??
                    'Server error. Please try again later.',
            };
        }
    }

    return {
        errorCode: 'INTERNAL_ERROR',
        message: 'Server error. Please try again later.',
    };
};

class BracketService {
    async getBracket(
        tournamentId: string,
    ): Promise<IBracketStructure | null> {
        try {
            const response =
                await axiosInstance.get<IBracketStructure>(
                    `/tournaments/${tournamentId}/bracket`,
                );

            return response.data;
        } catch (error) {
            if (
                axios.isAxiosError(error) &&
                error.response?.status === 409
            ) {
                return null;
            }

            throw buildBracketApiError(error);
        }
    }
}

export const bracketService =
    new BracketService();