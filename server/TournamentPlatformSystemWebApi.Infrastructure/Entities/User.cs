using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserModel
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool? IsOrganizer { get; set; }

    public Guid AccountStateId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual AccountStateModel AccountState { get; set; } = null!;

    public virtual ICollection<TournamentModel> Tournaments { get; set; } = new List<TournamentModel>();

    public virtual UserDetailModel? UserDetail { get; set; }

    public virtual ICollection<UserPhoneModel> UserPhones { get; set; } = new List<UserPhoneModel>();

    public virtual ICollection<UserTeamModel> UserTeams { get; set; } = new List<UserTeamModel>();
}
