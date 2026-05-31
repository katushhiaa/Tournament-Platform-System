using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TournamentPlatformSystemWebApi.Application.Interfaces;

namespace TournamentPlatformSystemWebApi.Infrastructure.Services;

public class UserPreferencesService : IUserPreferencesService
{
    private readonly IUserRepository _userRepository;
    private readonly IThemeRepository _themeRepository;

    public UserPreferencesService(IUserRepository userRepository, IThemeRepository themeRepository)
    {
        _userRepository = userRepository;
        _themeRepository = themeRepository;
    }

    public async Task<bool> GetPreferencesSetupCompletedAsync(Guid userId)
    {
        var completed = await _userRepository.GetPreferencesSetupCompletedAsync(userId);
        if (completed == null)
        {
            throw new KeyNotFoundException("User details not found");
        }

        return completed.Value;
    }

    public async Task MarkPreferencesSetupCompletedAsync(Guid userId)
    {
        var updated = await _userRepository.SetPreferencesSetupCompletedAsync(userId, true);
        if (!updated)
        {
            throw new KeyNotFoundException("User details not found");
        }
    }

    public async Task UpdateUserThemePreferencesAsync(Guid userId, IReadOnlyCollection<Guid> themeIds)
    {
        if (!await _userRepository.UserExistsAsync(userId))
        {
            throw new KeyNotFoundException("User not found");
        }

        var ids = themeIds?.Distinct().ToList() ?? new List<Guid>();
        if (!await _themeRepository.AreThemeIdsValidAsync(ids))
        {
            throw new ValidationException("One or more theme ids are invalid");
        }

        await _userRepository.SetUserThemePreferencesAsync(userId, ids);
    }
}
