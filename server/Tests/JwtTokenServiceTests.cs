using System;
using System.IdentityModel.Tokens.Jwt;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Tests
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void GenerateToken_IncludesExpectedClaimsAndExpiry()
        {
            // Arrange
            var key = "super-secret-key-that-is-long-enough-1234567890-ABCDEFG";
            var issuer = "test-issuer";
            var expirationMinutes = 60;
            var options = new JwtTokenOptions(key, issuer, expirationMinutes);
            var svc = new JwtTokenService(options);

            var userId = Guid.NewGuid();
            var email = "user@example.com";
            var role = "organizer";
            var isOrganizer = true;

            // Act
            var (tokenString, jwtId, expiresAt) = svc.GenerateToken(userId, email, role, isOrganizer);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            // Assert: claims
            Assert.Equal(userId.ToString(), token.Payload.Sub);
            Assert.Equal(email, token.Payload[JwtRegisteredClaimNames.Email]);
            Assert.Equal(role, token.Payload[System.Security.Claims.ClaimTypes.Role]);
            Assert.Equal(isOrganizer.ToString(), token.Payload["isOrganizer"]?.ToString());

            // Assert: issuer/audience
            Assert.Equal(issuer, token.Issuer);
            Assert.Contains(issuer, token.Audiences);

            // Assert: expiry approximately matches configured expiration
            var now = DateTime.UtcNow;
            var validTo = token.ValidTo;
            var diff = validTo - now;
            Assert.InRange(diff.TotalMinutes, expirationMinutes - 1, expirationMinutes + 1);
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsUniqueNonEmptyValues()
        {
            // Arrange
            var options = new JwtTokenOptions("k", "i", 1);
            var svc = new JwtTokenService(options);

            // Act
            var r1 = svc.GenerateRefreshToken();
            var r2 = svc.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(r1));
            Assert.False(string.IsNullOrWhiteSpace(r2));
            Assert.NotEqual(r1, r2);
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var key = "super-secret-key-that-is-long-enough-1234567890";
            var issuer = "test-issuer";
            var options = new JwtTokenOptions(key, issuer, 30);
            var svc = new JwtTokenService(options);

            var userId = Guid.NewGuid();
            var email = "valid@example.com";
            var role = "player";

            var (token, jwtId2, expiresAt2) = svc.GenerateToken(userId, email, role, false);

            // Act
            var principal = svc.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var mail = principal.FindFirst(ClaimTypes.Email)?.Value;
            var r = principal.FindFirst(ClaimTypes.Role)?.Value ?? principal.FindFirst("role")?.Value;
            Assert.Equal(userId.ToString(), sub);
            Assert.Equal(email, mail);
            Assert.Equal(role, r);
        }

        [Fact]
        public void ValidateToken_ExpiredToken_ThrowsSecurityTokenExpiredException()
        {
            // Arrange: token already expired (expirationMinutes = -1)
            var key = "another-super-secret-key-which-is-long-1234567890-XYZ";
            var issuer = "test-issuer";
            var options = new JwtTokenOptions(key, issuer, -1);
            var svc = new JwtTokenService(options);

            var (tokenExpired, jwtId3, expiresAt3) = svc.GenerateToken(Guid.NewGuid(), "a@b.com", "player", false);

            // Act & Assert
            Assert.Throws<SecurityTokenExpiredException>(() => svc.ValidateToken(tokenExpired));
        }

        [Fact]
        public void ValidateToken_InvalidSignature_ThrowsSecurityTokenInvalidSignatureException()
        {
            // Arrange: token created with keyA, validated with keyB
            var keyA = "key-A-which-is-long-enough-to-sign-AAAAAAAAAAAAAA";
            var keyB = "key-B-which-is-also-long-and-different-BBBBBBBBBBBBBB";
            var issuer = "test-issuer";
            var svcA = new JwtTokenService(new JwtTokenOptions(keyA, issuer, 30));
            var svcB = new JwtTokenService(new JwtTokenOptions(keyB, issuer, 30));

            var (tokenA, jA, eA) = svcA.GenerateToken(Guid.NewGuid(), "x@y.com", "player", false);

            // Act & Assert: validation with wrong key should throw invalid signature
            Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => svcB.ValidateToken(tokenA));
        }
    }
}
