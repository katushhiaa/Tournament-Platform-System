using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<TeamModel, Team>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.TournamentId, o => o.MapFrom(s => s.TournamentId))
            .ForMember(d => d.IsDisqualified, o => o.MapFrom(s => s.IsDisqualified))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UpdatedAt, o => o.MapFrom(s => s.UpdatedAt));

        CreateMap<Team, TeamModel>()
            .ForMember(d => d.MatchTeamAs, o => o.Ignore())
            .ForMember(d => d.MatchTeamBs, o => o.Ignore())
            .ForMember(d => d.MatchWinners, o => o.Ignore())
            .ForMember(d => d.Tournament, o => o.Ignore())
            .ForMember(d => d.UserTeams, o => o.Ignore());
    }
}
