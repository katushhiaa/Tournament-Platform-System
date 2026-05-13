using System;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentRepository : IRepository<Tournament, Guid>
{
    Task<bool> IsTitleUniqueAsync(string title, Guid organizerId);
    Task<bool> UpdateStatus(Guid id, TournamentStatus status);
    Task<int> GetParticipantsCountAsync(Guid tournamentId);
    Task<IReadOnlyList<Team>> GetTeamsAsync(Guid tournamentId);
    Task AddMatchesAsync(IEnumerable<TournamentPlatformSystemWebApi.Core.Entities.Match> matches);
    Task<IReadOnlyList<TournamentPlatformSystemWebApi.Core.Entities.Match>> GetMatchesAsync(Guid tournamentId);
}
