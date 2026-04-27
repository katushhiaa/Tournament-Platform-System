using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserDetailModel : BaseDbEntity
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
