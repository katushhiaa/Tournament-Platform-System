import axios from 'axios';
import axiosInstance from '../api/axiosInstance';

import type { IApiError } from '../types/Auth';

import type {
    IThemeOption,
    ITournament,
    ITournamentCreate,
    ITournamentResponse,
    ITournamentUpdate,
} from '../types/Tournament';

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

const buildTournamentApiError = (
    error: unknown,
): IApiError => {
    if (axios.isAxiosError<BackendErrorResponse>(error)) {
        const status = error.response?.status;
        const message =
            error.response?.data?.error?.message;

        if (status === 400) {
            return {
                errorCode: 'VALIDATION_ERROR',
                message:
                    message ??
                    'Invalid tournament data.',
            };
        }

        if (status === 401) {
            return {
                errorCode: 'UNAUTHORIZED',
                message:
                    message ??
                    'You must be authorized.',
            };
        }

        if (status === 403) {
            return {
                errorCode: 'ACCESS_FORBIDDEN',
                message:
                    message ??
                    'Access forbidden.',
            };
        }

        if (status === 404) {
            return {
                errorCode: 'NOT_FOUND',
                message:
                    message ??
                    'Tournament not found.',
            };
        }

        if (status === 409) {
            return {
                errorCode: 'CONFLICT',
                message:
                    message ??
                    'Tournament conflict.',
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
        message:
            'Server error. Please try again later.',
    };
};

const buildRegistrationUrl = (
    tournamentId: string,
): string => {
    return `${window.location.origin}/join/${tournamentId}`;
};

export const tournamentService = {
    async createTournament(
        data: ITournamentCreate,
    ): Promise<ITournamentResponse> {
        try {
            const response =
                await axiosInstance.post<ITournament>(
                    '/tournaments',
                    data,
                );

            return {
                ...response.data,
                registrationUrl:
                    buildRegistrationUrl(
                        response.data.id,
                    ),
            };
        } catch (error) {
            throw buildTournamentApiError(error);
        }
    },

    async uploadTournamentImage(
        tournamentId: string,
        formData: FormData,
    ): Promise<void> {
        await axiosInstance.post(
            `/tournaments/${tournamentId}/image`,
            formData,
            {
                headers: {
                    'Content-Type':
                        'multipart/form-data',
                },
            },
        );
    },

    async getTournamentById(
        id: string,
    ): Promise<ITournament> {
        try {

            if (import.meta.env.DEV) {
                console.log(
                    '[Tournament API] GET tournament:',
                    id,
                )
            }

            const response =
                await axiosInstance.get<ITournament>(
                    `/tournaments/${id}`,
                )

            return response.data

        } catch (error) {

            if (axios.isAxiosError(error)) {

                const status = error.response?.status

                if (status === 401) {

                    localStorage.setItem(
                        'redirectAfterLogin',
                        window.location.pathname,
                    )

                    window.location.href = '/login'

                    throw {
                        errorCode: 'UNAUTHORIZED',
                        message: 'Unauthorized',
                    }
                }

                if (status === 404) {

                    window.location.href = '/404'

                    throw {
                        errorCode: 'NOT_FOUND',
                        message: 'Tournament not found',
                    }
                }
            }

            throw buildTournamentApiError(error)
        }
    },

    async updateTournament(
        id: string,
        data: ITournamentUpdate,
    ): Promise<ITournament> {

        try {

            if (import.meta.env.DEV) {
                console.log(
                    '[Tournament API] PATCH tournament:',
                    id,
                    data,
                )
            }

            const response =
                await axiosInstance.patch<ITournament>(
                    `/tournaments/${id}`,
                    data,
                )

            return response.data

        } catch (error) {

            if (axios.isAxiosError(error)) {

                const status = error.response?.status

                if (status === 401) {

                    localStorage.setItem(
                        'redirectAfterLogin',
                        window.location.pathname,
                    )

                    window.location.href = '/login'

                    throw {
                        errorCode: 'UNAUTHORIZED',
                        message: 'Unauthorized',
                    }
                }

                if (status === 404) {

                throw {
                    errorCode: 'NOT_FOUND',
                    message: 'Tournament not found',
                }
            }

            if (status === 409) {

                throw {
                    errorCode: 'CONFLICT',
                    message:
                        'Editing unavailable. Tournament already active.',
                }
            }
        }

        throw buildTournamentApiError(error)
    }
},

    async startTournament(
        id: string,
    ): Promise<string> {
        try {
            const response =
                await axiosInstance.post<string>(
                    `/tournaments/${id}/start`,
                );

            return response.data;
        } catch (error) {
            throw buildTournamentApiError(error);
        }
    },

        async getSports(): Promise < IThemeOption[] > {
            try {
                const response =
                    await axiosInstance.get<IThemeOption[]>(
                        '/sport',
                    );

                return response.data;
            } catch(error) {
                throw buildTournamentApiError(error);
            }
        },
};