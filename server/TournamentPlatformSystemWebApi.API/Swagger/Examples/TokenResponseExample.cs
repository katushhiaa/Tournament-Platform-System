using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TokenResponseExample : IExamplesProvider<TokensResponseDto>
    {
        public TokensResponseDto GetExamples()
        {
            return new TokensResponseDto
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
                RefreshToken = "123124784y19472hjskbfkjsdcqsuycghqbf9uoikjca"

            };
        }
    }
}
