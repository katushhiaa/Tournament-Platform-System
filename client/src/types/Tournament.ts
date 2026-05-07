export interface ITournament {
    id: string;
    title: string;
    description: string | null;
    conditions: string | null;
    startDate: string;
    endDate: string;
    registrationCloseDate: string;
    sport: string;
    maxParticipants: number;
    status: string;
    organizerId: string;
}

export interface ITournamentCreate {
    title: string;
    description: string | null;
    conditions: string | null;
    startDate: string;
    endDate: string;
    registrationCloseDate: string;
    sport: string;
    maxParticipants: number;
}

export interface ITournamentResponse extends ITournament {
    registrationUrl: string;
}

export interface IThemeOption {
    id: string;
    name: string;
}