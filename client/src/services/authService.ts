import type { IAuthResponse, IRegisterRequest } from '../types/Auth';

const TAKEN_EMAILS = ['taken@example.com', 'admin@example.com', 'test@test.com'];

const STORAGE_KEY = 'mock_users';

const wait = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

const getStoredUsers = (): IRegisterRequest[] => {
    const data = localStorage.getItem(STORAGE_KEY);
    return data ? JSON.parse(data) : [];
};

const saveUser = (user: IRegisterRequest) => {
    const users = getStoredUsers();
    users.push(user);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(users));
};

class AuthService {
    async checkEmailUnique(email: string): Promise<boolean> {
        await wait(700);

        const normalizedEmail = email.trim().toLowerCase();

        const storedUsers = getStoredUsers();

        const existsInStorage = storedUsers.some(
            (u) => u.email.trim().toLowerCase() === normalizedEmail
        );

        return !TAKEN_EMAILS.includes(normalizedEmail) && !existsInStorage;
    }

    async register(data: IRegisterRequest): Promise<IAuthResponse> {
        await wait(1200);

        const normalizedEmail = data.email.trim().toLowerCase();

        const storedUsers = getStoredUsers();

        const existsInStorage = storedUsers.some(
            (u) => u.email.trim().toLowerCase() === normalizedEmail
        );

        if (TAKEN_EMAILS.includes(normalizedEmail) || existsInStorage) {
            throw {
                errorCode: 'EMAIL_TAKEN',
                message: 'Email already registered',
            };
        }

        if (normalizedEmail === 'server-error@example.com') {
            throw {
                errorCode: 'INTERNAL_ERROR',
                message: 'Server error. Please try again later.',
            };
        }

        //Зберігаємо користувача
        saveUser(data);

        return {
            userId: crypto.randomUUID(),
            fullName: data.fullName,
            email: data.email,
            role: data.role,
            token: 'mock-jwt-token',
        };
    }
}

export const authService = new AuthService();