using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystem.Application.DTOs.Auth;
using Xunit;

namespace TournamentPlatformSystemWebApi.Tests
{
    public class UserRepositoryTests
    {
        private TournamentdbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TournamentdbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new TournamentdbContext(options);
        }

        [Fact]
        public async Task CreateAsync_InsertsUserAndDetails()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);
            // ensure default account state exists
            ctx.AccountStates.Add(new AccountStateModel { Id = Guid.NewGuid(), Name = "active" });
            await ctx.SaveChangesAsync();

            var repo = new UserRepository(ctx);
            var userDto = new UserDto { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" };

            await repo.CreateAsync(userDto);

            var userModel = await ctx.Users.FindAsync(userDto.Id);
            Assert.NotNull(userModel);

            var detail = await ctx.UserDetails.FirstOrDefaultAsync(d => d.Email == "test@example.com");
            Assert.NotNull(detail);
            Assert.Equal(userDto.Id, detail.UserId);
        }

        [Fact]
        public async Task ExistsByEmailAsync_ReturnsTrueWhenExists()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);
            var userId = Guid.NewGuid();
            ctx.Users.Add(new UserModel { Id = userId, FullName = "U", PasswordHash = "h", AccountStateId = Guid.NewGuid() });
            ctx.UserDetails.Add(new UserDetailModel { Id = Guid.NewGuid(), UserId = userId, Email = "exists@example.com", DateOfBirth = DateOnly.MinValue });
            await ctx.SaveChangesAsync();

            var repo = new UserRepository(ctx);
            var exists = await repo.ExistsByEmailAsync("exists@example.com");
            Assert.True(exists);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsUserDto()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);
            var userId = Guid.NewGuid();
            ctx.Users.Add(new UserModel { Id = userId, FullName = "Anna", PasswordHash = "h", AccountStateId = Guid.NewGuid() });
            ctx.UserDetails.Add(new UserDetailModel { Id = Guid.NewGuid(), UserId = userId, Email = "anna@example.com", DateOfBirth = DateOnly.MinValue });
            await ctx.SaveChangesAsync();

            var repo = new UserRepository(ctx);
            var dto = await repo.GetByEmailAsync("anna@example.com");
            Assert.NotNull(dto);
            Assert.Equal(userId, dto!.Id);
            Assert.Equal("Anna", dto.Name);
            Assert.Equal("anna@example.com", dto.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUserWithPhonesInStats()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);
            var userId = Guid.NewGuid();
            ctx.Users.Add(new UserModel { Id = userId, FullName = "Bob", PasswordHash = "h", AccountStateId = Guid.NewGuid() });
            ctx.UserDetails.Add(new UserDetailModel { Id = Guid.NewGuid(), UserId = userId, Email = "bob@example.com", DateOfBirth = DateOnly.FromDateTime(DateTime.Parse("1990-01-01")) });
            ctx.UserPhones.Add(new UserPhoneModel { Id = Guid.NewGuid(), UserId = userId, PhoneNumber = "+123" });
            ctx.UserPhones.Add(new UserPhoneModel { Id = Guid.NewGuid(), UserId = userId, PhoneNumber = "+456" });
            await ctx.SaveChangesAsync();

            var repo = new UserRepository(ctx);
            var dto = await repo.GetByIdAsync(userId);
            Assert.NotNull(dto);
            Assert.Equal("Bob", dto!.Name);
            Assert.Equal("bob@example.com", dto.Email);
            var stats = dto.Stats as dynamic;
            Assert.NotNull(stats);
            var phones = stats.Phones as System.Collections.Generic.List<string>;
            Assert.Contains("+123", phones);
            Assert.Contains("+456", phones);
        }
    }
}
