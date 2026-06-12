using System;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using Xunit;

namespace Tests
{
    public class BcryptPasswordHasherTests
    {
        [Fact]
        public void HashPassword_ProducesDifferentHash_And_VerifySucceeds()
        {
            var hasher = new BcryptPasswordHasher(10);

            var pwd = "Secur3P@ssw0rd";
            var hash = hasher.HashPassword(pwd);

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.NotEqual(pwd, hash);
            Assert.True(hasher.VerifyPassword(pwd, hash));
        }

        [Fact]
        public void VerifyPassword_ReturnsFalse_ForIncorrectPassword()
        {
            var hasher = new BcryptPasswordHasher(10);

            var pwd = "CorrectP4ss";
            var wrong = "WrongP4ss";

            var hash = hasher.HashPassword(pwd);

            Assert.False(hasher.VerifyPassword(wrong, hash));
        }
    }
}
