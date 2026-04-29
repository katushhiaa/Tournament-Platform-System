import type {
    IApiError,
    IAuthResponse,
    ILoginRequest,
    IRegisterRequest,
    IUserProfileResponse,
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

const parseErrorBody = async (response: Response): Promise<BackendErrorResponse> => {
    try {
        return (await response.json()) as BackendErrorResponse;
    } catch {
        return {};
    }
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
                const errorBody = await parseErrorBody(response);

                let errorCode = 'INTERNAL_ERROR';

                if (response.status === 400) {
                    errorCode = 'VALIDATION_ERROR';
                } else if (response.status === 409) {
                    errorCode = 'EMAIL_TAKEN';
                } else if (response.status >= 500) {
                    errorCode = 'INTERNAL_ERROR';
                } else if (errorBody.error?.type) {
                    errorCode = errorBody.error.type;
                }

                throw {
                    errorCode,
                    message: errorBody.error?.message ?? 'Server error. Please try again later.',
                } satisfies IApiError;
            }

            const successBody = (await response.json()) as Partial<IAuthResponse> &
                Partial<BackendTokenOnlyResponse>;

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
            if (import.meta.env.DEV) {
                console.error('[authService] register error', error);
            }

            const apiError = error as Partial<IApiError>;

            throw {
                errorCode: apiError.errorCode ?? 'INTERNAL_ERROR',
                message: apiError.message ?? 'Server error. Please try again later.',
            } satisfies IApiError;
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
            const loginResponse = await fetch('/api/v1/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                },
                body: JSON.stringify(payload),
            });

            if (!loginResponse.ok) {
                const errorBody = await parseErrorBody(loginResponse);

                let errorCode = 'INTERNAL_ERROR';

                if (loginResponse.status === 400) {
                    errorCode = 'VALIDATION_ERROR';
                } else if (loginResponse.status === 401) {
                    errorCode = 'INVALID_CREDENTIALS';
                } else if (loginResponse.status === 403) {
                    errorCode = 'ACCOUNT_LOCKED';
                } else if (loginResponse.status === 429) {
                    errorCode = 'TOO_MANY_ATTEMPTS';
                } else if (loginResponse.status >= 500) {
                    errorCode = 'INTERNAL_ERROR';
                }

                throw {
                    errorCode,
                    message: errorBody.error?.message ?? 'Login failed. Please try again.',
                } satisfies IApiError;
            }

            const loginResult = (await loginResponse.json()) as BackendTokenOnlyResponse;

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

            const profile = (await profileResponse.json()) as IUserProfileResponse;

            const result: IAuthResponse = {
                userId: profile.id,
                email: profile.email,
                fullName: profile.name,
                role: normalizeRole(profile.role),
                token: loginResult.token,
                refreshToken: null,
            };

            if (import.meta.env.DEV) {
                console.log('[authService] login response', result);
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

    async checkEmailUnique(email: string): Promise<boolean> {
        if (import.meta.env.DEV) {
            console.log('[authService] checkEmailUnique fallback', email);
        }

        return true;
    }
}

export const authService = new AuthService();