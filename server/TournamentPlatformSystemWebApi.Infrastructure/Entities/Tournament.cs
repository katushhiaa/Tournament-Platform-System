using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class TournamentModel : BaseDbEntity
{
    public string Name { get; set; } = null!;

    public Guid? OrganizerId { get; set; }

    public Guid ThemeId { get; set; }

    public int MaxTeams { get; set; }

    public string? BackgroundImg { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime RegistrationDeadline { get; set; }

    public DateTime EndDate { get; set; }

    public string? Description { get; set; }

    public string? Conditions { get; set; }

    public TournamentStatusType Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MatchModel> Matches { get; set; } = new List<MatchModel>();

    public virtual UserModel? Organizer { get; set; }

    public virtual ICollection<TeamModel> Teams { get; set; } = new List<TeamModel>();

    public virtual TournamentThemeModel Theme { get; set; } = null!;
}
