
namespace TournamentPlatformSystemWebApi.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<DTOs.UserSearchItemResponce>> GetUsersWithQueryAsync(string query);

}