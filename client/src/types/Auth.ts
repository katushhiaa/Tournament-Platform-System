export type UserRole = 'organizer' | 'player';

export interface IRegisterRequest {
    fullName: string;
    phoneNumber: string;
    email: string;
    dateOfBirth: string;
    role: UserRole;
    password: string;
}

export interface IRegisterFormValues extends IRegisterRequest {
    confirmPassword: string;
}

export interface IUser {
    userId: string;
    fullName: string;
    email: string;
    role: UserRole;
}

export interface IAuthResponse {
    userId: string;
    fullName: string;
    email: string;
    role: UserRole;
    token: string;
    refreshToken?: string | null;
}

export interface IApiError {
    errorCode: string;
    message: string;
}

export interface ILoginFormValues {
    email: string;
    password: string;
    rememberMe: boolean;
}

export interface ILoginRequest {
    email: string;
    password: string;
}

export interface ILoginResponse {
    tokens: {
        accessToken: string;
        refreshToken: string;
    };
    user: {
        id: string;
        email: string;
        fullName: string;
        role: string;
    };
}

export interface IUserProfileResponse {
    id: string;
    email: string;
    name: string;
    role: string;
    stats?: string;
}