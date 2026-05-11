using System;
using AutoMapper;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class TournamentThemeProfile : Profile
{
    public TournamentThemeProfile()
    {
        CreateMap<TournamentThemeModel, TournamentTheme>();

        CreateMap<TournamentTheme, TournamentThemeModel>();
    }
}

