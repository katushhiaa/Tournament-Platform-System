using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IThemeRepository : IRepository<TournamentTheme, Guid>
{
    Task<bool> IsSportWithId(Guid id);
    Task<bool> AreThemeIdsValidAsync(IReadOnlyCollection<Guid> ids);
}
