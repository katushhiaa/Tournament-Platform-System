using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class TournamentProfile : Profile
{
    public TournamentProfile()
    {
        CreateMap<TournamentModel, Tournament>()
            .ForMember(d => d.OrganizerName, o => o.MapFrom(s => s.Organizer != null ? s.Organizer.FullName : null))
            .ForMember(d => d.ThemeName, o => o.MapFrom(s => s.Theme != null ? s.Theme.Name : null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status));

        CreateMap<Tournament, TournamentModel>()
            .ForMember(d => d.Theme, o => o.Ignore())
            .ForMember(d => d.Organizer, o => o.Ignore())
            .ForMember(d => d.Matches, o => o.Ignore())
            .ForMember(d => d.Teams, o => o.Ignore())
            .ForMember(d => d.ThemeId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.ThemeId, o => o.MapFrom(x => x.ThemeId))
            .ForMember(d => d.StartDate, o => o.MapFrom(x => DateTime.SpecifyKind(x.StartDate, DateTimeKind.Unspecified)))
            .ForMember(d => d.EndDate, o => o.MapFrom(x => DateTime.SpecifyKind(x.EndDate, DateTimeKind.Unspecified)))
            .ForMember(d => d.RegistrationDeadline, o => o.MapFrom(x => DateTime.SpecifyKind(x.RegistrationDeadline, DateTimeKind.Unspecified)));
    }
}
