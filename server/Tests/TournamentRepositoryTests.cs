using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Mappings;
using TournamentPlatformSystemWebApi.Infrastructure.Repositories;
using Xunit;

namespace Tests
{
    public class TournamentRepositoryTests
    {
        private readonly IMapper _mapper;

        public TournamentRepositoryTests()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.AddProfile<TournamentProfile>();
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
        public async Task CreateAsync_AddsTournament_ReturnsId()
        {
            using var context = CreateInMemoryContext();
            var repo = new TournamentRepository(context, _mapper);

            var tournament = new Tournament
            {
                Name = "Cup",
                OrganizerId = Guid.NewGuid(),
                Status = TournamentStatus.REGISTRATION_OPEN
            };

            var id = await repo.CreateAsync(tournament);

            var db = await context.Tournaments.FindAsync(id);

            Assert.NotNull(db);
            Assert.Equal("Cup", db!.Name);
            Assert.Equal(tournament.OrganizerId, db.OrganizerId);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedTournament_WithOrganizerAndTheme()
        {
            using var context = CreateInMemoryContext();

            var organizer = new UserModel { Id = Guid.NewGuid(), FullName = "Org Name", PasswordHash = "testpassword" };
            var theme = new TournamentThemeModel { Id = Guid.NewGuid(), Name = "ThemeX" };

            var tid = Guid.NewGuid();
            context.Add(organizer);
            context.Add(theme);
            context.Tournaments.Add(new TournamentModel
            {
                Id = tid,
                Name = "T1",
                OrganizerId = organizer.Id,
                Organizer = organizer,
                Theme = theme,
                ThemeId = theme.Id,
                Status = TournamentStatusType.IN_PROGRESS
            });

            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var res = await repo.GetByIdAsync(tid);

            Assert.NotNull(res);
            Assert.Equal("T1", res!.Name);
            Assert.Equal("Org Name", res.OrganizerName);
            Assert.Equal("ThemeX", res.ThemeName);
            Assert.Equal(TournamentStatus.IN_PROGRESS, res.Status);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedTournaments()
        {
            using var context = CreateInMemoryContext();

            var theme = new TournamentThemeModel { Id = Guid.NewGuid(), Name = "T" };
            context.Add(theme);
            context.Tournaments.AddRange(
                new TournamentModel { Id = Guid.NewGuid(), Name = "A", Theme = theme, ThemeId = theme.Id, Status = TournamentStatusType.REGISTRATION_OPEN },
                new TournamentModel { Id = Guid.NewGuid(), Name = "B", Theme = theme, ThemeId = theme.Id, Status = TournamentStatusType.REGISTRATION_OPEN }
            );
            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var res = await repo.GetAllAsync();

            Assert.Equal(2, res.Count);
            Assert.Contains(res, r => r.Name == "A" && r.ThemeName == "T");
            Assert.Contains(res, r => r.Name == "B" && r.ThemeName == "T");
        }

        [Fact]
        public async Task UpdateAsync_UpdatesTournament_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var id = Guid.NewGuid();
            context.Tournaments.Add(new TournamentModel { Id = id, Name = "Old", Status = TournamentStatusType.REGISTRATION_OPEN });
            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var toUpdate = new Tournament { Id = id, Name = "New" };

            var updated = await repo.UpdateAsync(toUpdate);

            Assert.Equal("New", updated.Name);
            var db = await context.Tournaments.FindAsync(id);
            Assert.Equal("New", db!.Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesTournament_WhenExists()
        {
            using var context = CreateInMemoryContext();
            var id = Guid.NewGuid();
            context.Tournaments.Add(new TournamentModel { Id = id, Name = "ToDelete", Status = TournamentStatusType.REGISTRATION_OPEN });
            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var result = await repo.DeleteAsync(id);

            Assert.True(result);
            var db = await context.Tournaments.FindAsync(id);
            Assert.Null(db);
        }

        [Fact]
        public async Task IsTitleUniqueAsync_ReturnsFalse_WhenDuplicateExists()
        {
            using var context = CreateInMemoryContext();
            var orgId = Guid.NewGuid();
            context.Tournaments.Add(new TournamentModel { Id = Guid.NewGuid(), Name = "SameTitle", OrganizerId = orgId, Status = TournamentStatusType.REGISTRATION_OPEN });
            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var res = await repo.IsTitleUniqueAsync("sAMeTitle", orgId);

            Assert.False(res);
        }

        [Fact]
        public async Task IsTitleUniqueAsync_ReturnsTrue_WhenUniqueOrDifferentOrganizer()
        {
            using var context = CreateInMemoryContext();
            var orgA = Guid.NewGuid();
            var orgB = Guid.NewGuid();
            context.Tournaments.Add(new TournamentModel { Id = Guid.NewGuid(), Name = "TitleX", OrganizerId = orgA, Status = TournamentStatusType.REGISTRATION_OPEN });
            await context.SaveChangesAsync();

            var repo = new TournamentRepository(context, _mapper);

            var res1 = await repo.IsTitleUniqueAsync("Different", orgA);
            var res2 = await repo.IsTitleUniqueAsync("TitleX", orgB);

            Assert.True(res1);
            Assert.True(res2);
        }

        [Fact]
        public async Task IsTitleUniqueAsync_ReturnsTrue_WhenTitleEmpty()
        {
            using var context = CreateInMemoryContext();
            var repo = new TournamentRepository(context, _mapper);

            Assert.True(await repo.IsTitleUniqueAsync("   ", Guid.NewGuid()));
            Assert.True(await repo.IsTitleUniqueAsync(string.Empty, Guid.NewGuid()));
            Assert.True(await repo.IsTitleUniqueAsync(null!, Guid.NewGuid()));
        }
    }
}
