using System;
using System.Collections.Generic;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserPhoneModel : BaseDbEntity
{
    public Guid UserId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
