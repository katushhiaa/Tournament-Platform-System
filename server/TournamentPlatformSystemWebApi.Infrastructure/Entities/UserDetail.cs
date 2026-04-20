using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserDetailModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
