using System.Text.Json.Serialization;

namespace TournamentPlatformSystemWebApi.Application.DTOs.Auth
{
    public class TokensResponseDto
    {
        public string? AccessToken { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
    }
}
