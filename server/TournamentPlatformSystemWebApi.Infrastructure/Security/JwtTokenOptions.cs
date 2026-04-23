namespace TournamentPlatformSystemWebApi.Infrastructure.Security
{
    public record JwtTokenOptions(string Key, string Issuer, int ExpirationDays = 1);
}
