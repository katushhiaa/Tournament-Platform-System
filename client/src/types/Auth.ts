export interface IRegisterRequest {
    fullName: string;
    phoneNumber: string;
    email: string;
    dateOfBirth: string;
    role: 'organizer' | 'player' | 'Organizer' | 'Player';
    password: string;
}

export interface IUser {
    userId: string;
    fullName: string;
    email: string;
    role: string;
}

export interface IAuthResponse {
    userId: string;
    fullName: string;
    email: string;
    role: string;
    token: string;
}

export interface IRegisterFormValues {
    fullName: string;
    phoneNumber: string;
    email: string;
    dateOfBirth: string;
    role: 'organizer' | 'player';
    password: string;
    confirmPassword: string;
}

export interface IApiError {
    errorCode: string;
    message: string;
}