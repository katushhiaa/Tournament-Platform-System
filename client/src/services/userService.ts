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
}