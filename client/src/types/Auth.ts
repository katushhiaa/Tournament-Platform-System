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
}

export interface IApiError {
    errorCode: string;
    message: string;
}