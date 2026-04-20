using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserPhoneModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
