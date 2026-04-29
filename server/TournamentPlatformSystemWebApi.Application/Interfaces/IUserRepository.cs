using System;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> GetUserWithDetails(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<string?> GetPasswordHashByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);


}
