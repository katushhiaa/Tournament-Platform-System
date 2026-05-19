using System;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Core.Entities;

public class UserTournamentThemePreference : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ThemeId { get; set; }
    public DateTime? CreatedAt { get; set; }
}