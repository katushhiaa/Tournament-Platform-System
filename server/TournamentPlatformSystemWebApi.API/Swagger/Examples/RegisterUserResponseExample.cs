using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class RegisterUserResponseExample : IExamplesProvider<RegisterUserResponse>
{
    public RegisterUserResponse GetExamples()
    {
        return new RegisterUserResponse
        {
            UserId = Guid.Empty,
            Email = "user email",
            FullName = "user name",
            Role = "user role",
            Tokens = new TokensResponseDto
            {
                AccessToken = "some access token",
                RefreshToken = "some refresh token"
            }
        };
    }

}
