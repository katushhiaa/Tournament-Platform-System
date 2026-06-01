import type { IBracketStructure } from './Bracket'

export interface ITournament {
    id: string

    title: string

    description: string | null

    conditions: string | null

    startDate: string

    endDate: string

    registrationCloseDate: string

    sportId: string

    sportName?: string

    maxParticipants: number

    participantsCount?: number

    status: string

    organizerId: string

    organizerName?: string

    backgroundImg?: string | null

    matches?: IBracketStructure
}

export interface ITournamentCreate {
    title: string

    description: string | null

    conditions: string | null

    startDate: string

    endDate: string

    registrationCloseDate: string

    sport: string

    maxParticipants: number
}

export interface ITournamentUpdate
    extends Partial<ITournamentCreate> { }

export interface ITournamentResponse
    extends ITournament {
    registrationUrl: string
}

export interface ITournamentPreview {
    id: string
    title: string
    status: string
    backgroundImg: string | null
    sportName: string
    startDate: string
    participantsCount: number
    maxParticipants: number
}

export interface IThemeOption {
    id: string
    name: string
    imageUrl?: string | null
}