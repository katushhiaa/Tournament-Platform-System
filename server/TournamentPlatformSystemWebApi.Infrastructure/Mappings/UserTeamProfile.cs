using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class UserTeamProfile : Profile
{
    public UserTeamProfile()
    {
        CreateMap<UserTeamModel, UserTeam>()
            .ForMember(d => d.TeamName, o => o.MapFrom(s => s.Team != null ? s.Team.Name : null))
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? s.User.FullName : null))
            .ForMember(d => d.CreatedAt, o => o.MapFrom(s => s.CreatedAt));

        CreateMap<UserTeam, UserTeamModel>()
            .ForMember(d => d.Team, o => o.Ignore())
            .ForMember(d => d.User, o => o.Ignore());
    }
}
