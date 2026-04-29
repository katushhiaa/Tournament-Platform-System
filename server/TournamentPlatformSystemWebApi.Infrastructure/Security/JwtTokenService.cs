using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace TournamentPlatformSystemWebApi.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtTokenOptions _options;

        public JwtTokenService(JwtTokenOptions options)
        {
            _options = options;
        }

        public string GenerateToken(Guid userId, string email, string role, bool isOrganizer)
        {
            var key = _options.Key ?? "default-development-key-change-in-production";
            var issuer = _options.Issuer ?? "tournament-api";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role ?? string.Empty),
                new Claim("isOrganizer", isOrganizer.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            // Use base64url-friendly string
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_options.Key ?? "default-development-key-change-in-production");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer ?? "tournament-api",
                ValidateAudience = true,
                ValidAudience = _options.Issuer ?? "tournament-api",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return principal;
        }
    }
}
