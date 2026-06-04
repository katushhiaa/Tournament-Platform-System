using System;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentRepository : IRepository<Tournament, Guid>
{
    Task<bool> IsTitleUniqueAsync(string title, Guid organizerId);
    Task<bool> UpdateStatus(Guid id, TournamentStatus status);
    Task<int> GetParticipantsCountAsync(Guid tournamentId);
    Task<IReadOnlyList<Team>> GetTeamsAsync(Guid tournamentId);
    Task<bool> IsUserInTournamentAsync(Guid tournamentId, Guid userId);
    Task<bool> IsTeamNameUsedAsync(Guid tournamentId, string teamName);
    Task<Team> AddParticipantAsync(Guid tournamentId, Guid userId, string teamName);
    Task<bool> RemoveParticipantAsync(Guid tournamentId, Guid userId);
    Task<bool> DisqualifyParticipantAsync(Guid tournamentId, Guid userId);
    Task AddMatchesAsync(IEnumerable<TournamentPlatformSystemWebApi.Core.Entities.Match> matches);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Core.Entities.Match>> GetMatchesAsync(Guid tournamentId);
    Task<TournamentPlatformSystemWebApi.Core.Entities.Match> UpdateMatchAsync(TournamentPlatformSystemWebApi.Core.Entities.Match match);
    Task<IReadOnlyList<TournamentPreviewDto>> GetForUserAsync(Guid userId, int page, int pageSize, IReadOnlyList<TournamentStatus>? statuses);
    Task<IReadOnlyList<TournamentPreviewDto>> GetAllPreviewAsync(int page, int pageSize, bool randomize, IReadOnlyList<TournamentStatus>? statuses, string? searchQuery = null);
}
