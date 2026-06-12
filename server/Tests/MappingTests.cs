using System;
using System.Collections.Generic;
using AutoMapper;
using Xunit;
using TournamentPlatformSystemWebApi.Infrastructure.Entities;
using TournamentPlatformSystemWebApi.Core.Entities;
using TournamentPlatformSystemWebApi.Infrastructure.Mappings;

namespace Tests;

public class MappingTests
{
    private MapperConfiguration CreateConfig()
    {
        return new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserProfile).Assembly);
        });
    }

    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = CreateConfig();
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_UserModel_To_User()
    {
        var config = CreateConfig();
        var mapper = config.CreateMapper();

        var userModel = new UserModel
        {
            Id = Guid.NewGuid(),
            FullName = "John Doe",
            AccountState = new AccountStateModel { Description = "Active", IsActive = true },
            UserDetail = new UserDetailModel { Id = Guid.NewGuid(), Email = "a@b.com", DateOfBirth = DateOnly.Parse("1990-01-01") },
            UserPhones = new List<UserPhoneModel> { new() { PhoneNumber = "+123" } }
        };

        var user = mapper.Map<User>(userModel);

        Assert.Equal(userModel.Id, user.Id);
        Assert.Equal(userModel.FullName, user.FullName);
        Assert.Equal(userModel.AccountState.Description, user.AccountStateDescription);
        Assert.True(user.IsActive);
        Assert.Equal(userModel.UserDetail.Email, user.UserDetail!.Email);
        Assert.Contains("+123", user.UserDetail!.Phones);
    }

    [Fact]
    public void Map_TournamentModel_To_Tournament()
    {
        var config = CreateConfig();
        var mapper = config.CreateMapper();

        var tournamentModel = new TournamentModel
        {
            Id = Guid.NewGuid(),
            Name = "Cup",
            MaxTeams = 16,
            Theme = new TournamentThemeModel { Id = Guid.NewGuid(), Name = "ThemeA" },
            Organizer = new UserModel { FullName = "Org" },
            Status = TournamentStatusType.IN_PROGRESS
        };

        var t = mapper.Map<Tournament>(tournamentModel);

        Assert.Equal(tournamentModel.Id, t.Id);
        Assert.Equal("Cup", t.Name);
        Assert.Equal("Org", t.OrganizerName);
        Assert.Equal("ThemeA", t.ThemeName);
        Assert.Equal(TournamentStatus.IN_PROGRESS, t.Status);
    }

    [Fact]
    public void Map_TeamModel_To_Team()
    {
        var config = CreateConfig();
        var mapper = config.CreateMapper();

        var teamModel = new TeamModel { Id = Guid.NewGuid(), Name = "T1", TournamentId = Guid.NewGuid(), IsDisqualified = false, CreatedAt = DateTime.UtcNow };
        var team = mapper.Map<Team>(teamModel);

        Assert.Equal(teamModel.Id, team.Id);
        Assert.Equal(teamModel.Name, team.Name);
        Assert.Equal(teamModel.TournamentId, team.TournamentId);
    }

    [Fact]
    public void Map_MatchModel_To_Match()
    {
        var config = CreateConfig();
        var mapper = config.CreateMapper();

        var matchModel = new MatchModel
        {
            Id = Guid.NewGuid(),
            TeamA = new TeamModel { Name = "A" },
            TeamB = new TeamModel { Name = "B" },
            TeamAScore = 1,
            TeamBScore = 2
        };

        var m = mapper.Map<Match>(matchModel);

        Assert.Equal(matchModel.Id, m.Id);
        Assert.Equal("A", m.TeamAName);
        Assert.Equal("B", m.TeamBName);
        Assert.Equal(1, m.TeamAScore);
    }

    [Fact]
    public void Map_UserTeamModel_To_UserTeam()
    {
        var config = CreateConfig();
        var mapper = config.CreateMapper();

        var utModel = new UserTeamModel { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Team = new TeamModel { Name = "TeamX" }, User = new UserModel { FullName = "UserX" } };
        var ut = mapper.Map<UserTeam>(utModel);

        Assert.Equal(utModel.Id, ut.Id);
        Assert.Equal("TeamX", ut.TeamName);
        Assert.Equal("UserX", ut.UserName);
    }
}
