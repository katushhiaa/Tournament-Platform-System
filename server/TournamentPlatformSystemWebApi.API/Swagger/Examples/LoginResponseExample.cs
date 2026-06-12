using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class LoginResponseExample : IExamplesProvider<LoginResponseDto>
{
    public LoginResponseDto GetExamples()
    {
        return new LoginResponseDto
        {
            Tokens = new TokensResponseDto
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "123124784y19472hjskbfkjsdcqsuycghqbf9uoikjca"
            },
            User = new UserDto
            {
                Email = "example@com.com",
                Id = Guid.NewGuid(),
                Role = "organizer",
                FullName = "User Fullname"
            }

        };
    }
}
