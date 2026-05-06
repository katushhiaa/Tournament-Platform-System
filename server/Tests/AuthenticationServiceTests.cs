using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Exceptions;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using TournamentPlatformSystemWebApi.Infrastructure.Services;
using Xunit;

namespace Tests
{
    public class AuthenticationServiceTests
    {
        private TournamentdbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TournamentdbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TournamentdbContext(options);
        }



        [Fact]
        public async Task RegisterAsync_Success_CallsRepositoryAndReturnsToken()
        {
            var userId = Guid.NewGuid();
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            repoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(userId);
            repoMock.Setup(r => r.SetRefreshTokenForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            using var db = CreateInMemoryContext();
            var hasherMock = new Mock<IPasswordHasher>();
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(j => j.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(("test-token", "test-jti", DateTime.UtcNow.AddMinutes(60)));

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "Auth:RefreshTokenDays", "7" } }).Build();
            var svc = new AuthenticationService(repoMock.Object, hasherMock.Object, jwtMock.Object, configuration);

            var req = new RegisterUserRequest
            {
                Email = "unit@test.local",
                Password = "GoodP4ss1",
                FullName = "Unit Test",
                PhoneNumber = "+380123456789",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)),
                Role = "Player"
            };

            var res = await svc.RegisterAsync(req);

            Assert.Equal(userId, res.UserId);
            Assert.Equal(req.Email, res.Email);
            Assert.False(string.IsNullOrWhiteSpace(res.Tokens?.AccessToken));
            repoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateEmail_Throws()
        {
            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            repoMock.Setup(r => r.SetRefreshTokenForUser(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            using var db = CreateInMemoryContext();
            var hasherMock = new Mock<IPasswordHasher>();
            var jwtMock = new Mock<IJwtTokenService>();
            jwtMock.Setup(j => j.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Returns(("test-token", "test-jti", DateTime.UtcNow.AddMinutes(60)));

            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "Auth:RefreshTokenDays", "7" } }).Build();
            var svc = new AuthenticationService(repoMock.Object, hasherMock.Object, jwtMock.Object, configuration);

            var req = new RegisterUserRequest
            {
                Email = "dup@test.local",
                Password = "GoodP4ss1",
                FullName = "Unit Test",
                PhoneNumber = "+380123456789",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)),
                Role = "Player"
            };

            await Assert.ThrowsAsync<DuplicateEmailException>(() => svc.RegisterAsync(req));
        }
    }
}
