using System;
using System.Security.Claims;

namespace TournamentPlatformSystemWebApi.Infrastructure.Security
{
    public interface IJwtTokenService
    {
        (string Token, string JwtId, DateTime ExpiresAt) GenerateToken(Guid userId, string email, string role, bool isOrganizer);
        ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken();
    }
}
