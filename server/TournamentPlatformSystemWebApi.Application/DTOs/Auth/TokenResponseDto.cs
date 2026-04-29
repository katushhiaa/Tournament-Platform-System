namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class TokensResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
