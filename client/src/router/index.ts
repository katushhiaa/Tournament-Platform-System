import { createRouter, createWebHistory } from 'vue-router';
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
    ],
});

router.beforeEach((to) => {
    const isAuthenticated = authStore.state.isAuthenticated;
    const role = authStore.state.user?.role;

    if (to.meta.requiresAuth && !isAuthenticated) {
        return '/register';
    }

    if (to.meta.guestOnly && isAuthenticated && role) {
        return authStore.getDashboardRouteByRole(role);
    }

    if (to.meta.requiresAuth && to.meta.role && role && to.meta.role !== role) {
        return authStore.getDashboardRouteByRole(role);
    }

    return true;
});

export default router;