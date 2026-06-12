using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class TournamentThemeModel : BaseDbEntity
{
    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TournamentModel> Tournaments { get; set; } = new List<TournamentModel>();
    public virtual ICollection<UserTournamentThemePreferenceModel> UserPreferences { get; set; } = new List<UserTournamentThemePreferenceModel>();
}
