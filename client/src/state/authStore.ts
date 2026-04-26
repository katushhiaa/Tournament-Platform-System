import { computed, reactive } from 'vue';
import type { IAuthResponse, IUser } from '../types/Auth';

type AuthState = {
    token: string | null;
    isAuthenticated: boolean;
    user: IUser | null;
};

const AUTH_TOKEN_KEY = 'zvytiaha_token';
const AUTH_USER_KEY = 'zvytiaha_user';

const storedToken = localStorage.getItem(AUTH_TOKEN_KEY);
const storedUser = localStorage.getItem(AUTH_USER_KEY);

const normalizeRole = (role: string): 'organizer' | 'player' => {
    return role.toLowerCase() === 'organizer' ? 'organizer' : 'player';
};

const state = reactive<AuthState>({
    token: storedToken,
    isAuthenticated: Boolean(storedToken),
    user: storedUser
        ? {
            ...(JSON.parse(storedUser) as IUser),
            role: normalizeRole((JSON.parse(storedUser) as IUser).role),
        }
        : null,
});

const setAuth = (payload: IAuthResponse) => {
    state.token = payload.token;
    state.isAuthenticated = true;
    state.user = {
        userId: payload.userId,
        fullName: payload.fullName,
        email: payload.email,
        role: normalizeRole(payload.role),
    };

    localStorage.setItem(AUTH_TOKEN_KEY, payload.token);
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify(state.user));
};

const clearAuth = () => {
    state.token = null;
    state.isAuthenticated = false;
    state.user = null;

    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(AUTH_USER_KEY);
};

const getDashboardRouteByRole = (role: string) => {
    return normalizeRole(role) === 'organizer' ? '/organizer/dashboard' : '/player/dashboard';
};

const dashboardRoute = computed(() => {
    if (!state.user) return '/';
    return getDashboardRouteByRole(state.user.role);
});

export const authStore = {
    state,
    setAuth,
    clearAuth,
    getDashboardRouteByRole,
    dashboardRoute,
};