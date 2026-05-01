import { defineStore } from 'pinia';
import { authService } from '../services/authService';
import type { IAuthResponse, ILoginRequest, IUser, UserRole } from '../types/Auth';

type AuthState = {
    currentUser: IUser | null;
    token: string | null;
    refreshToken: string | null;
    isLoading: boolean;
    error: string | null;
};

const AUTH_TOKEN_KEY = 'zvytiaha_token';
const AUTH_REFRESH_TOKEN_KEY = 'zvytiaha_refresh_token';
const AUTH_USER_KEY = 'zvytiaha_user';

const normalizeRole = (role: string): UserRole => {
    return role.toLowerCase() === 'organizer' ? 'organizer' : 'player';
};

export const useAuthStore = defineStore('auth', {
    state: (): AuthState => ({
        currentUser: null,
        token: null,
        refreshToken: null,
        isLoading: false,
        error: null,
    }),

    getters: {
        isAuthenticated: (state) => Boolean(state.token),
        isOrganizer: (state) => state.currentUser?.role === 'organizer',
        isPlayer: (state) => state.currentUser?.role === 'player',
        userName: (state) => state.currentUser?.fullName ?? '',
    },

    actions: {
        initializeAuth() {
            const storedToken = localStorage.getItem(AUTH_TOKEN_KEY);
            const storedRefreshToken = localStorage.getItem(AUTH_REFRESH_TOKEN_KEY);
            const storedUser = localStorage.getItem(AUTH_USER_KEY);

            this.token = storedToken;
            this.refreshToken = storedRefreshToken;

            if (storedUser) {
                const parsedUser = JSON.parse(storedUser) as IUser;

                this.currentUser = {
                    ...parsedUser,
                    role: normalizeRole(parsedUser.role),
                };
            }
        },

        setToken(token: string, refreshToken?: string | null) {
            this.token = token;
            localStorage.setItem(AUTH_TOKEN_KEY, token);

            this.refreshToken = refreshToken ?? null;

            if (refreshToken) {
                localStorage.setItem(AUTH_REFRESH_TOKEN_KEY, refreshToken);
            } else {
                localStorage.removeItem(AUTH_REFRESH_TOKEN_KEY);
            }
        },

        setUser(user: IUser) {
            this.currentUser = {
                ...user,
                role: normalizeRole(user.role),
            };

            localStorage.setItem(AUTH_USER_KEY, JSON.stringify(this.currentUser));
        },

        setCurrentUser(user: IUser) {
            this.setUser(user);
        },

        setAuth(payload: IAuthResponse) {
            this.setToken(payload.token, payload.refreshToken ?? null);

            this.setUser({
                userId: payload.userId,
                fullName: payload.fullName,
                email: payload.email,
                role: normalizeRole(payload.role),
            });

            this.error = null;
        },

        async login(credentials: ILoginRequest) {
            this.isLoading = true;
            this.error = null;

            try {
                const response = await authService.login(credentials);
                this.setAuth(response);
                return response;
            } catch (error) {
                const apiError = error as { message?: string };
                this.error = apiError.message ?? 'Login failed';
                throw error;
            } finally {
                this.isLoading = false;
            }
        },

        logout() {
            this.clearAuth();
        },

        clearAuth() {
            this.currentUser = null;
            this.token = null;
            this.refreshToken = null;
            this.isLoading = false;
            this.error = null;

            localStorage.removeItem(AUTH_TOKEN_KEY);
            localStorage.removeItem(AUTH_REFRESH_TOKEN_KEY);
            localStorage.removeItem(AUTH_USER_KEY);
        },

        getDashboardRouteByRole(role: string) {
            return normalizeRole(role) === 'organizer'
                ? '/organizer/dashboard'
                : '/player/dashboard';
        },
    },
});