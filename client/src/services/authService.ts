import { authStore } from '../state/authStore';
import type {
    IApiError,
    IAuthResponse,
    ILoginRequest,
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
            const response = await fetch('/api/v1/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                let errorBody: BackendErrorResponse = {};

                try {
                    errorBody = (await response.json()) as BackendErrorResponse;
                } catch {
                    errorBody = {};
                }

                let errorCode = 'INTERNAL_ERROR';
                const errorMessage = errorBody.error?.message ?? 'Server error. Please try again later.';

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
                userId: typeof successBody.userId === 'string' ? successBody.userId : crypto.randomUUID(),
                email: typeof successBody.email === 'string' ? successBody.email : payload.email,
                fullName:
                    typeof successBody.fullName === 'string' ? successBody.fullName : payload.fullName,
                role: normalizeRole(typeof successBody.role === 'string' ? successBody.role : payload.role),
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

    async login(data: ILoginRequest): Promise<IAuthResponse> {
        const payload = {
            email: data.email.trim(),
            password: data.password,
        };

        if (import.meta.env.DEV) {
            console.log('[authService] login request', { email: payload.email });
        }

        try {
            const loginResponse = await fetch('/api/v1/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                },
                body: JSON.stringify(payload),
            });

            if (!loginResponse.ok) {
                let errorBody: BackendErrorResponse = {};

                try {
                    errorBody = (await loginResponse.json()) as BackendErrorResponse;
                } catch {
                    errorBody = {};
                }

                let errorCode = 'INTERNAL_ERROR';

                if (loginResponse.status === 400) {
                    errorCode = 'VALIDATION_ERROR';
                } else if (loginResponse.status === 401) {
                    errorCode = 'INVALID_CREDENTIALS';
                } else if (loginResponse.status === 423) {
                    errorCode = 'ACCOUNT_LOCKED';
                } else if (loginResponse.status >= 500) {
                    errorCode = 'INTERNAL_ERROR';
                }

                throw {
                    errorCode,
                    message: errorBody.error?.message ?? 'Login failed. Please try again.',
                } satisfies IApiError;
            }

            const loginResult = (await loginResponse.json()) as { token: string };

            if (!loginResult.token) {
                throw {
                    errorCode: 'INVALID_RESPONSE',
                    message: 'Token is missing in server response.',
                } satisfies IApiError;
            }

            const profileResponse = await fetch('/api/v1/users/me', {
                method: 'GET',
                headers: {
                    Authorization: `Bearer ${loginResult.token}`,
                    Accept: 'application/json',
                },
            });

            if (!profileResponse.ok) {
                throw {
                    errorCode: 'PROFILE_LOAD_ERROR',
                    message: 'Unable to load user profile.',
                } satisfies IApiError;
            }

            const profile = (await profileResponse.json()) as {
                id: string;
                email: string;
                name: string;
                role: string;
            };

            const result: IAuthResponse = {
                userId: profile.id,
                email: profile.email,
                fullName: profile.name,
                role: profile.role.toLowerCase() === 'organizer' ? 'organizer' : 'player',
                token: loginResult.token,
            };

            authStore.setAuth(result);

            if (import.meta.env.DEV) {
                console.log('[authService] login successful', result);
            }

            return result;
        } catch (error) {
            if (import.meta.env.DEV) {
                console.error('[authService] login error', error);
            }

            const apiError = error as Partial<IApiError>;

            throw {
                errorCode: apiError.errorCode ?? 'INTERNAL_ERROR',
                message: apiError.message ?? 'Server error. Please try again later.',
            } satisfies IApiError;
        }
    }
}

export const authService = new AuthService();
