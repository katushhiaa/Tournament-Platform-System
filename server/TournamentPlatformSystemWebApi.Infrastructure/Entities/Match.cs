using System;
using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class MatchModel
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid TeamAId { get; set; }

    public Guid? TeamBId { get; set; }

    public Guid? WinnerId { get; set; }

    public int Level { get; set; }

    public int OrderNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public int? TeamAScore { get; set; }

    public int? TeamBScore { get; set; }

    public bool? IsValid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TeamModel TeamA { get; set; } = null!;

    public virtual TeamModel? TeamB { get; set; }

    public virtual TournamentModel Tournament { get; set; } = null!;

    public virtual TeamModel? Winner { get; set; }
}
