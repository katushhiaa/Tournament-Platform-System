using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using Xunit;

namespace Tests
{
    public class BaseRepositoryTests
    {
        private readonly IMapper _mapper;

        public BaseRepositoryTests()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<UserModel, User>()
                    .ForMember(d => d.AccountStateDescription, o => o.MapFrom(s => s.AccountState != null ? s.AccountState.Description : null));
                c.CreateMap<UserDetailModel, UserDetail>();
                c.CreateMap<User, UserModel>()
                    .ForMember(d => d.AccountState, o => o.Ignore())
                    .ForMember(d => d.PasswordHash, o => o.MapFrom(_ => string.Empty))
                    .ForMember(d => d.UserPhones, o => o.Ignore())
                    .ForMember(d => d.Tournaments, o => o.Ignore())
                    .ForMember(d => d.UserTeams, o => o.Ignore());
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
        public async Task CreateAsync_AddsEntity_ReturnsId()
        {
            using var context = CreateInMemoryContext();
            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var user = new User { FullName = "Test Create" };

            var id = await repo.CreateAsync(user);

            var db = await context.Users.FindAsync(id);

            Assert.NotNull(db);
            Assert.Equal("Test Create", db!.FullName);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedEntities()
        {
            using var context = CreateInMemoryContext();

            var accountState = new AccountStateModel { Id = Guid.NewGuid(), Name = "ActiveState", Description = "Active" };
            context.Add(accountState);

            context.Users.AddRange(new UserModel { Id = Guid.NewGuid(), FullName = "A", AccountState = accountState, PasswordHash = "h" }, new UserModel { Id = Guid.NewGuid(), FullName = "B", AccountState = accountState, PasswordHash = "h" });
            await context.SaveChangesAsync();

            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var results = await repo.GetAllAsync();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.FullName == "A" && r.AccountStateDescription == "Active");
            Assert.Contains(results, r => r.FullName == "B" && r.AccountStateDescription == "Active");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedEntity_WhenFound()
        {
            using var context = CreateInMemoryContext();
            var id = Guid.NewGuid();
            context.Users.Add(new UserModel { Id = id, FullName = "Found", PasswordHash = "h" });
            await context.SaveChangesAsync();

            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var res = await repo.GetByIdAsync(id);

            Assert.NotNull(res);
            Assert.Equal("Found", res!.FullName);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEntity_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var id = Guid.NewGuid();
            context.Users.Add(new UserModel { Id = id, FullName = "Old", PasswordHash = "h" });
            await context.SaveChangesAsync();

            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var toUpdate = new User { Id = id, FullName = "New" };

            var updated = await repo.UpdateAsync(toUpdate);

            Assert.Equal("New", updated.FullName);
            var db = await context.Users.FindAsync(id);
            Assert.Equal("New", db!.FullName);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntity_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var id = Guid.NewGuid();
            context.Users.Add(new UserModel { Id = id, FullName = "ToDelete", PasswordHash = "h" });
            await context.SaveChangesAsync();

            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var result = await repo.DeleteAsync(id);

            Assert.True(result);
            var db = await context.Users.FindAsync(id);
            Assert.Null(db);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            using var context = CreateInMemoryContext();
            var repo = new BaseRepository<User, UserModel>(context, _mapper);

            var result = await repo.DeleteAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }
}
