import { createRouter, createWebHistory } from 'vue-router';
import { h } from 'vue';
import HomePage from '../views/HomePage.vue';
import RegisterPage from '../views/RegisterPage.vue';
import LoginPage from '../views/LoginPage.vue';
import OrganizerDashboardPage from '../views/OrganizerDashboardPage.vue';
import PlayerDashboardPage from '../views/PlayerDashboardPage.vue';
import { authStore } from '../state/authStore';

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
    const isAuthenticated = authStore.state.isAuthenticated;
    const role = authStore.state.user?.role;

    if (to.meta.requiresAuth && !isAuthenticated) {
        return '/register';
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