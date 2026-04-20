using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class StatusProfile : Profile
{
    public StatusProfile()
    {
        CreateMap<TournamentStatusType, TournamentStatus>().ReverseMap();
    }
}
