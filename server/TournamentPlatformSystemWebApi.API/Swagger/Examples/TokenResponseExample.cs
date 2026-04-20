using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TokenResponseExample : IExamplesProvider<TokenResponseDto>
    {
        public TokenResponseDto GetExamples()
        {
            return new TokenResponseDto
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            };
        }
    }
}
