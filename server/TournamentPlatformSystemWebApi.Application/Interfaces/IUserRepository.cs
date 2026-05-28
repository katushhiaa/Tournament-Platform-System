using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> GetUserWithDetails(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<string?> GetPasswordHashByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> UserExistsAsync(Guid userId);
    Task<bool?> GetPreferencesSetupCompletedAsync(Guid userId);
    Task<bool> SetPreferencesSetupCompletedAsync(Guid userId, bool value);
    Task SetUserThemePreferencesAsync(Guid userId, IReadOnlyCollection<Guid> themeIds);
    Task SetRefreshTokenForUser(Guid userId, string token, string jwtId, DateTime expiresAt);
    Task<bool> ValidateRefreshTokenForUser(Guid userId, string token, string jwtId);
    Task RevokeUserTokens(Guid userId);

    Task<IReadOnlyList<UserSearchItemResponce>> SearchUsersAsync(string query, int limit = 20);

}
