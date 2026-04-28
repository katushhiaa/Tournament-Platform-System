using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class AccountStateModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserModel> Users { get; set; } = new List<UserModel>();
}
