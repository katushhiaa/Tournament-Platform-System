using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using TournamentPlatformSystemWebApi.Infrastructure.Security;
using Xunit;

namespace Tests
{
    public class TestPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) => "HASH:" + password;
        public bool VerifyPassword(string password, string hash) => hash == HashPassword(password);
    }

    public class UserRepositoryTests
    {
        private readonly IMapper _mapper;

        public UserRepositoryTests()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<User, UserModel>()
                    .ForMember(d => d.PasswordHash, o => o.Ignore())
                    .ForMember(d => d.AccountState, o => o.Ignore())
                    .ForMember(d => d.UserPhones, o => o.Ignore())
                    .ForMember(d => d.Tournaments, o => o.Ignore())
                    .ForMember(d => d.UserTeams, o => o.Ignore());
                c.CreateMap<UserDetail, UserDetailModel>();
            });

            _mapper = cfg.CreateMapper();
        }

        private TournamentdbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TournamentdbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TournamentdbContext(options);
        }

        [Fact]
        public async Task CreateAsync_CreatesUserDetailAndPhoneAndHashesPassword()
        {
            using var context = CreateInMemoryContext();

            // ensure active account state exists so repository resolves it
            var activeState = new AccountStateModel { Id = Guid.NewGuid(), Name = "active", Description = "Active" };
            context.Add(activeState);
            await context.SaveChangesAsync();

            var hasher = new TestPasswordHasher();
            var repo = new UserRepository(context, _mapper, hasher);

            var user = new User
            {
                FullName = "Repo Test",
                Password = "StrongP4ss",
                IsOrganizer = true,
                UserDetail = new UserDetail
                {
                    Email = "repo@test.local",
                    DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25)),
                    Phones = new List<string> { "+380123456789" }
                }
            };

            var id = await repo.CreateAsync(user);

            Assert.NotEqual(Guid.Empty, id);

            var dbUser = await context.Users.FindAsync(id);
            Assert.NotNull(dbUser);
            Assert.Equal("Repo Test", dbUser!.FullName);
            Assert.True(hasher.VerifyPassword(user.Password, dbUser.PasswordHash));

            var detail = await context.UserDetails.FirstOrDefaultAsync(d => d.UserId == id);
            Assert.NotNull(detail);
            Assert.Equal("repo@test.local", detail!.Email);

            var phone = await context.UserPhones.FirstOrDefaultAsync(p => p.UserId == id);
            Assert.NotNull(phone);
            Assert.Equal("+380123456789", phone!.PhoneNumber);
        }

        [Fact]
        public async Task ExistsByEmailAsync_FindsExistingEmail()
        {
            using var context = CreateInMemoryContext();

            var userId = Guid.NewGuid();
            context.UserDetails.Add(new UserDetailModel { Id = Guid.NewGuid(), UserId = userId, Email = "ex@e.com", DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)) });
            await context.SaveChangesAsync();

            var repo = new UserRepository(context, _mapper, new TestPasswordHasher());

            var exists = await repo.ExistsByEmailAsync("ex@e.com");

            Assert.True(exists);
        }
    }
}
