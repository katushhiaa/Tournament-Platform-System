import { h } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../views/HomePage.vue';
import RegisterPage from '../views/RegisterPage.vue';
import LoginPage from '../views/LoginPage.vue';
import OrganizerDashboardPage from '../views/OrganizerDashboardPage.vue';
import PlayerDashboardPage from '../views/PlayerDashboardPage.vue';
import { useAuthStore } from '../stores/authStore';

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomePage,
        },
        {
            path: '/register',
            name: 'register',
            component: RegisterPage,
            meta: { guestOnly: true },
        },
        {
            path: '/login',
            name: 'login',
            component: LoginPage,
            meta: { guestOnly: true },
        },
        {
            path: '/organizer/dashboard',
            name: 'organizer-dashboard',
            component: OrganizerDashboardPage,
            meta: { requiresAuth: true, role: 'organizer' },
        },
        {
            path: '/player/dashboard',
            name: 'player-dashboard',
            component: PlayerDashboardPage,
            meta: { requiresAuth: true, role: 'player' },
        },
        {
            path: '/tournaments',
            name: 'tournaments',
            component: {
                render() {
                    return h(
                        'main',
                        {
                            style:
                                'min-height:100vh;background:#252e35;color:#fffcf2;display:flex;align-items:center;justify-content:center;font-size:32px;',
                        },
                        'Tournaments page is under development',
                    );
                },
            },
        },
    ],
});

router.beforeEach((to) => {
    const authStore = useAuthStore();

    if (!authStore.token) {
        authStore.initializeAuth();
    }

    const isAuthenticated = authStore.isAuthenticated;
    const role = authStore.currentUser?.role;

    if (to.meta.requiresAuth && !isAuthenticated) {
        return {
            path: '/login',
            query: { redirect: to.fullPath },
        };
    }

    if (to.meta.guestOnly && isAuthenticated && role) {
        const dashboardRoute = authStore.getDashboardRouteByRole(role);

        if (to.path !== dashboardRoute) {
            return dashboardRoute;
        }

        return true;
    }

    if (to.meta.requiresAuth && to.meta.role && role) {
        const requiredRole = String(to.meta.role).toLowerCase();

        if (requiredRole !== role) {
            const dashboardRoute = authStore.getDashboardRouteByRole(role);

            if (to.path !== dashboardRoute) {
                return dashboardRoute;
            }
        }
    }

    return true;
});

export default router;