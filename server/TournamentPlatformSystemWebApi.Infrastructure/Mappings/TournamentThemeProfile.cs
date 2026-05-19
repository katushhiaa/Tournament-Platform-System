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

        CreateMap<TournamentTheme, TournamentThemeModel>()
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.Tournaments, o => o.Ignore())
            .ForMember(d => d.UserPreferences, o => o.Ignore());
    }
}

