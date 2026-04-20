using System;
using TournamentPlatformSystem.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserRepository
{

    Task CreateAsync(UserDto user);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
}
