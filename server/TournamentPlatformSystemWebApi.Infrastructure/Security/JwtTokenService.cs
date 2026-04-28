using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TournamentPlatformSystemWebApi.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtTokenOptions _options;

        public JwtTokenService(JwtTokenOptions options)
        {
            _options = options;
        }

        public string GenerateToken(Guid userId, string email, string role)
        {
            var key = _options.Key ?? "default-development-key-change-in-production";
            var issuer = _options.Issuer ?? "tournament-api";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_options.ExpirationDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
