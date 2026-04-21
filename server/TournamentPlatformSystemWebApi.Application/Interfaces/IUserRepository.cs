using System;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User> GetUserWithDetails(Guid id);
}
