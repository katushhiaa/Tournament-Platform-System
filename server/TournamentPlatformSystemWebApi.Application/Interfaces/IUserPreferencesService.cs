using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserPreferencesService
{
    Task<bool> GetPreferencesSetupCompletedAsync(Guid userId);
    Task MarkPreferencesSetupCompletedAsync(Guid userId);
    Task UpdateUserThemePreferencesAsync(Guid userId, IReadOnlyCollection<Guid> themeIds);
}
