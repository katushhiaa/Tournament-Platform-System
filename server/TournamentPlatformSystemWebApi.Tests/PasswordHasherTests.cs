using TournamentPlatformSystemWebApi.Infrastructure.Security;
using Xunit;

namespace TournamentPlatformSystemWebApi.Tests
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashAndVerify_Work()
        {
            var hasher = new BcryptPasswordHasher(10);
            var password = "My$ecretP@ss";
            var hash = hasher.HashPassword(password);
            Assert.NotNull(hash);
            Assert.True(hasher.VerifyPassword(password, hash));
            Assert.False(hasher.VerifyPassword("wrong", hash));
        }
    }
}
