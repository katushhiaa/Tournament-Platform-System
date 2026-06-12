using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class MatchProfile : Profile
{
    public MatchProfile()
    {
        CreateMap<MatchModel, Match>()
            .ForMember(d => d.TeamAName, o => o.MapFrom(s => s.TeamA != null ? s.TeamA.Name : null))
            .ForMember(d => d.TeamBName, o => o.MapFrom(s => s.TeamB != null ? s.TeamB.Name : null));

        CreateMap<Match, MatchModel>()
            .ForMember(d => d.TeamA, o => o.Ignore())
            .ForMember(d => d.TeamB, o => o.Ignore())
            .ForMember(d => d.Tournament, o => o.Ignore())
            .ForMember(d => d.Winner, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());
    }
}
