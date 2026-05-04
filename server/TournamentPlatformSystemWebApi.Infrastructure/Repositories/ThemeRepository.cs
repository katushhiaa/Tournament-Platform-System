using System;
using AutoMapper;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Repositories;

public class ThemeRepository : BaseRepository<TournamentTheme, TournamentThemeModel>, IThemeRepository
{
    public ThemeRepository(TournamentdbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}
