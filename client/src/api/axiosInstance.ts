import axios from 'axios';
import { useAuthStore } from '../stores/authStore';
import router from '../router';

// 👉 базовий URL (через proxy у vite)
const axiosInstance = axios.create({
    baseURL: '/api/v1',
    headers: {
        'Content-Type': 'application/json',
    },
});

// 🔹 REQUEST INTERCEPTOR
axiosInstance.interceptors.request.use((config) => {
    const authStore = useAuthStore();

    if (authStore.token) {
        config.headers.Authorization = `Bearer ${authStore.token}`;
    }

    return config;
});

// 🔹 RESPONSE INTERCEPTOR
axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        const authStore = useAuthStore();

        if (error.response) {
            const status = error.response.status;

            // ❗ 401 → logout + redirect
            if (status === 401) {
                authStore.clearAuth();
                router.push('/login');
            }

            // ❗ 403 → доступ заборонений
            if (status === 403) {
                alert('Доступ заборонений');
            }
        }

        return Promise.reject(error);
    },
);

export default axiosInstance;