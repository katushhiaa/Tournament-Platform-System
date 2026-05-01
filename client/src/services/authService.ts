import axios from 'axios';
import axiosInstance from '../api/axiosInstance';
import type {
    IApiError,
    IAuthResponse,
    ILoginRequest,
    ILoginResponse,
    IRegisterRequest,
    UserRole,
} from '../types/Auth';

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

type BackendTokenOnlyResponse = {
    token: string;
};

const normalizeRole = (role: string): UserRole => {
    return role.toLowerCase() === 'organizer' ? 'organizer' : 'player';
};

const toBackendRole = (role: string): 'Organizer' | 'Player' => {
    return role.toLowerCase() === 'organizer' ? 'Organizer' : 'Player';
};

const buildApiError = (
    status: number | undefined,
    data: BackendErrorResponse | undefined,
    fallbackMessage: string,
): IApiError => {
    let errorCode = 'INTERNAL_ERROR';

    if (status === 400) {
        errorCode = 'VALIDATION_ERROR';
    } else if (status === 401) {
        errorCode = 'INVALID_CREDENTIALS';
    } else if (status === 403) {
        errorCode = 'ACCESS_FORBIDDEN';
    } else if (status === 409) {
        errorCode = 'EMAIL_TAKEN';
    } else if (status === 429) {
        errorCode = 'TOO_MANY_ATTEMPTS';
    } else if (status && status >= 500) {
        errorCode = 'INTERNAL_ERROR';
    } else if (data?.error?.type) {
        errorCode = data.error.type;
    }

    return {
        errorCode,
        message: data?.error?.message ?? fallbackMessage,
    };
};

const handleAxiosError = (error: unknown, fallbackMessage: string): IApiError => {
    if (axios.isAxiosError<BackendErrorResponse>(error)) {
        return buildApiError(error.response?.status, error.response?.data, fallbackMessage);
    }

    const apiError = error as Partial<IApiError>;

    return {
        errorCode: apiError.errorCode ?? 'INTERNAL_ERROR',
        message: apiError.message ?? fallbackMessage,
    };
};

class AuthService {
    async register(data: IRegisterRequest): Promise<IAuthResponse> {
        const payload = {
            email: data.email.trim(),
            password: data.password,
            fullName: data.fullName.trim(),
            phoneNumber: data.phoneNumber.trim(),
            dateOfBirth: data.dateOfBirth,
            role: toBackendRole(data.role),
        };

        if (import.meta.env.DEV) {
            console.log('[authService] register request', payload);
        }

        try {
            const response = await axiosInstance.post<
                Partial<IAuthResponse> & BackendTokenOnlyResponse
            >('/auth/register', payload);

            const successBody = response.data;

            if (!successBody.token) {
                throw {
                    errorCode: 'INVALID_RESPONSE',
                    message: 'Token is missing in server response.',
                } satisfies IApiError;
            }

            const result: IAuthResponse = {
                userId:
                    typeof successBody.userId === 'string'
                        ? successBody.userId
                        : crypto.randomUUID(),
                email: typeof successBody.email === 'string' ? successBody.email : payload.email,
                fullName:
                    typeof successBody.fullName === 'string'
                        ? successBody.fullName
                        : payload.fullName,
                role: normalizeRole(
                    typeof successBody.role === 'string' ? successBody.role : payload.role,
                ),
                token: successBody.token,
                refreshToken: successBody.refreshToken ?? null,
            };

            if (import.meta.env.DEV) {
                console.log('[authService] register response', result);
            }

            return result;
        } catch (error) {
            const apiError = handleAxiosError(error, 'Server error. Please try again later.');

            if (import.meta.env.DEV) {
                console.error('[authService] register error', apiError);
            }

            throw apiError;
        }
    }

    async login(data: ILoginRequest): Promise<IAuthResponse> {
        const payload = {
            email: data.email.trim(),
            password: data.password,
        };

        if (import.meta.env.DEV) {
            console.log('[authService] login request', { email: payload.email });
        }

        try {
            const loginResponse = await axiosInstance.post<ILoginResponse>('/auth/login', payload);

            const loginResult = loginResponse.data;

            if (!loginResult.tokens?.accessToken) {
                throw {
                    errorCode: 'INVALID_RESPONSE',
                    message: 'Access token is missing in server response.',
                } satisfies IApiError;
            }

            if (!loginResult.user) {
                throw {
                    errorCode: 'INVALID_RESPONSE',
                    message: 'User is missing in server response.',
                } satisfies IApiError;
            }

            const result: IAuthResponse = {
                userId: loginResult.user.id,
                email: loginResult.user.email,
                fullName: loginResult.user.fullName,
                role: normalizeRole(loginResult.user.role),
                token: loginResult.tokens.accessToken,
                refreshToken: loginResult.tokens.refreshToken ?? null,
            };

            if (import.meta.env.DEV) {
                console.log('[authService] login response', result);
            }

            return result;
        } catch (error) {
            const apiError = handleAxiosError(error, 'Server error. Please try again later.');

            if (import.meta.env.DEV) {
                console.error('[authService] login error', apiError);
            }

            throw apiError;
        }
    }

    async checkEmailUnique(email: string): Promise<boolean> {
        if (import.meta.env.DEV) {
            console.log('[authService] checkEmailUnique fallback', email);
        }

        return true;
    }
}

export const authService = new AuthService();