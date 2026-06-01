import type { ITournamentPreview } from '../types/Tournament'

export const getSportPreferences = (userId: string): string[] => {
  try {
    const raw = localStorage.getItem(`preferences_${userId}`)
    return raw ? JSON.parse(raw) : []
  } catch {
    return []
  }
}

export const sortByPreferences = (
  tournaments: ITournamentPreview[],
  preferredSportNames: string[],
): ITournamentPreview[] => {
  if (!preferredSportNames.length) return tournaments
  const preferred = new Set(preferredSportNames.map(n => n.toLowerCase()))
  return [...tournaments].sort((a, b) => {
    const aMatch = preferred.has(a.sportName?.toLowerCase() ?? '') ? 0 : 1
    const bMatch = preferred.has(b.sportName?.toLowerCase() ?? '') ? 0 : 1
    return aMatch - bMatch
  })
}