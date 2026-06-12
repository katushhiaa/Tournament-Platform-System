import axiosInstance from '../api/axiosInstance'

export interface IUserSearchItem {
    id: string
    fullName: string
}

export const userService = {
    async searchUsers(query?: string): Promise<IUserSearchItem[]> {
        const response = await axiosInstance.get<IUserSearchItem[]>('/users', {
            params: query ? { q: query } : {},
        })
        return response.data
    },

    async getOnboardingStatus(): Promise<{ preferencesSetupCompleted: boolean }> {
        const response = await axiosInstance.get('/users/me/onboarding-status')
        return response.data
    },

    async completeOnboarding(): Promise<void> {
        await axiosInstance.patch('/users/me/onboarding-complete')
    },

    async savePreferences(themeIds: string[]): Promise<void> {
        await axiosInstance.put('/users/me/preferences', { themeIds })
    },
}