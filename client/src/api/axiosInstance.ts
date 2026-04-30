import axios from 'axios';
import { useAuthStore } from '../stores/authStore';

const axiosInstance = axios.create({
    baseURL: '/api/v1',
    headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
    },
});

axiosInstance.interceptors.request.use((config) => {
    const authStore = useAuthStore();

    if (authStore.token) {
        config.headers.Authorization = `Bearer ${authStore.token}`;
    }

    return config;
});

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        const authStore = useAuthStore();
        const status = error.response?.status;

        if (status === 401) {
            authStore.clearAuth();

            if (window.location.pathname !== '/login') {
                window.location.href = '/login';
            }
        }

        if (status === 403) {
            alert('Доступ заборонений');
        }

        return Promise.reject(error);
    },
);

export default axiosInstance;