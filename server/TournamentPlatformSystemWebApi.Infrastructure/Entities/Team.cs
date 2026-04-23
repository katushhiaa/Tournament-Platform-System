using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class TeamModel : BaseDbEntity
{
    public string Name { get; set; } = null!;

    public Guid TournamentId { get; set; }

    public bool? IsDisqualified { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MatchModel> MatchTeamAs { get; set; } = new List<MatchModel>();

    public virtual ICollection<MatchModel> MatchTeamBs { get; set; } = new List<MatchModel>();

    public virtual ICollection<MatchModel> MatchWinners { get; set; } = new List<MatchModel>();

    public virtual TournamentModel Tournament { get; set; } = null!;

    public virtual ICollection<UserTeamModel> UserTeams { get; set; } = new List<UserTeamModel>();
}
