import { authStore } from '../state/authStore';
import type { IApiError, IAuthResponse, IRegisterRequest } from '../types/Auth';

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

class AuthService {
    async register(data: IRegisterRequest): Promise<IAuthResponse> {
        const payload = {
            email: data.email.trim(),
            password: data.password,
            fullName: data.fullName.trim(),
            phoneNumber: data.phoneNumber.trim(),
            dateOfBirth: data.dateOfBirth,
            role: data.role.toLowerCase() === 'organizer' ? 'Organizer' : 'Player',
        };

        if (import.meta.env.DEV) {
            console.log('[authService] register request', payload);
        }

        try {
            const response = await fetch('/api/v1/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                const errorBody = (await response.json()) as BackendErrorResponse;

                let errorCode = 'INTERNAL_ERROR';
                const errorMessage =
                    errorBody.error?.message ?? 'Server error. Please try again later.';

                if (response.status === 400) {
                    errorCode = 'VALIDATION_ERROR';
                } else if (response.status === 409) {
                    errorCode = 'EMAIL_TAKEN';
                } else if (response.status >= 500) {
                    errorCode = 'INTERNAL_ERROR';
                } else if (errorBody.error?.type) {
                    errorCode = errorBody.error.type;
                }

                const apiError: IApiError = {
                    errorCode,
                    message: errorMessage,
                };

                if (import.meta.env.DEV) {
                    console.error('[authService] error response', apiError);
                }

                throw apiError;
            }

            const successBody = (await response.json()) as Partial<IAuthResponse> &
                Partial<BackendTokenOnlyResponse>;

            if (import.meta.env.DEV) {
                console.log('[authService] response', successBody);
            }

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
                email:
                    typeof successBody.email === 'string' ? successBody.email : payload.email,
                fullName:
                    typeof successBody.fullName === 'string'
                        ? successBody.fullName
                        : payload.fullName,
                role:
                    typeof successBody.role === 'string' ? successBody.role : payload.role,
                token: successBody.token,
            };

            authStore.setAuth(result);

            if (import.meta.env.DEV) {
                console.log('[authService] normalized response', result);
            }

            return result;
        } catch (error) {
            if (import.meta.env.DEV) {
                console.error('[authService] network/runtime error', error);
            }

            const apiError = error as Partial<IApiError>;

            throw {
                errorCode: apiError.errorCode ?? 'INTERNAL_ERROR',
                message: apiError.message ?? 'Server error. Please try again later.',
            } satisfies IApiError;
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