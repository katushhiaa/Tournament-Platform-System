using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class LoginRequestExample : IExamplesProvider<LoginRequestDto>
    {
        public LoginRequestDto GetExamples()
        {
            return new LoginRequestDto
            {
                Email = "player@example.com",
                Password = "PlayerPass123"
            };
        }
    }
}
