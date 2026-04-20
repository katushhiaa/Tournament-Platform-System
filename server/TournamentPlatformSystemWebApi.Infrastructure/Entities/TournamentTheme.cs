using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class TournamentThemeModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TournamentModel> Tournaments { get; set; } = new List<TournamentModel>();
}
