using System;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IThemeRepository : IRepository<TournamentTheme, Guid>
{

}
