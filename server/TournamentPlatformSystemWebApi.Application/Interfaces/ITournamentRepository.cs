using System;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface ITournamentRepository : IRepository<Tournament, Guid>
{
    Task<bool> IsTitleUniqueAsync(string title, Guid organizerId);

}
