using AutoMapper;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;

namespace TournamentPlatformSystemWebApi.Infrastructure.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {

        // Convert UserPhoneModel -> string (phone number)
        CreateMap<UserPhoneModel, string>().ConvertUsing(src => src.PhoneNumber);

        // UserDetail mapping: populate Phones from related User.UserPhones when available
        CreateMap<UserDetailModel, UserDetail>()
            .ForMember(d => d.Phones, o => o.MapFrom(s => s.User != null ? s.User.UserPhones : new List<UserPhoneModel>()));

        // Main User mapping
        CreateMap<UserModel, User>()
            .ForMember(d => d.AccountStateDescription, o => o.MapFrom(s => s.AccountState != null ? s.AccountState.Description : null))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => s.AccountState != null ? s.AccountState.IsActive ?? false : false))
            .ForMember(d => d.UserDetail, o => o.MapFrom(s => s.UserDetail))
            .ForMember(d => d.Password, o => o.Ignore())
            // populate UserDetail.Phones from the flat UserPhones collection on the user
            .ForPath(d => d.UserDetail.Phones, o => o.MapFrom(s => s.UserPhones));

        // Reverse maps: ignore DB-only navigation properties where Core model lacks them
        CreateMap<User, UserModel>()
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.AccountState, o => o.Ignore())
            .ForMember(d => d.Tournaments, o => o.Ignore())
            .ForMember(d => d.UserPhones, o => o.Ignore())
            .ForMember(d => d.UserTeams, o => o.Ignore())
            .ForMember(d => d.UserDetail, o => o.Ignore())
            // Ignore DB-only scalar fields that Core.User doesn't have
            .ForMember(d => d.AccountStateId, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.DeletedAt, o => o.Ignore());
    }
}
