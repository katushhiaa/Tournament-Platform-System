using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserTeamModel : BaseDbEntity
{
    public Guid UserId { get; set; }

    public Guid TeamId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TeamModel Team { get; set; } = null!;

    public virtual UserModel User { get; set; } = null!;
}
