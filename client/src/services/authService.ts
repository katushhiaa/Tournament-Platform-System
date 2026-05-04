import axios from 'axios';
import axiosInstance from '../api/axiosInstance';
import type {
    IApiError,
    IAuthResponse,
    ILoginRequest,
    ILoginResponse,
    IRegisterRequest,
    IRegisterResponse,
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
    errors?: Record<string, string[] | string>;
};

const normalizeRole = (role: string): UserRole => {
    return role.toLowerCase() === 'organizer' ? 'organizer' : 'player';
};

const toBackendRole = (role: string): 'organizer' | 'player' => {
    return role.toLowerCase() === 'organizer' ? 'organizer' : 'player';
};

const normalizeFieldName = (field: string) => {
    const normalized = field.charAt(0).toLowerCase() + field.slice(1);

    if (normalized === 'phone') return 'phoneNumber';
    if (normalized === 'name') return 'fullName';

    return normalized;
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

    const fieldErrors: Record<string, string> = {};

    if (data?.errors) {
        Object.entries(data.errors).forEach(([field, value]) => {
            fieldErrors[normalizeFieldName(field)] = Array.isArray(value) ? value[0] : value;
        });
    }

    return {
        errorCode,
        message: data?.error?.message ?? fallbackMessage,
        fieldErrors: Object.keys(fieldErrors).length ? fieldErrors : undefined,
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
        fieldErrors: apiError.fieldErrors,
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
            const response = await axiosInstance.post<IRegisterResponse>('/auth/register', payload);
            const successBody = response.data;

            if (!successBody.tokens?.accessToken) {
                throw {
                    errorCode: 'INVALID_RESPONSE',
                    message: 'Access token is missing in server response.',
                } satisfies IApiError;
            }

            const result: IAuthResponse = {
                userId: successBody.userId,
                email: successBody.email,
                fullName: successBody.fullName,
                role: normalizeRole(successBody.role),
                token: successBody.tokens.accessToken,
                refreshToken: successBody.tokens.refreshToken ?? null,
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
            rememberMe: data.rememberMe,
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