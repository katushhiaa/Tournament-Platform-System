using System;
using TournamentPlatformSystemWebApi.Common.Helpers;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class UserTournamentThemePreferenceModel : BaseDbEntity
{
    public Guid UserId { get; set; }
    
    public Guid ThemeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual UserModel User { get; set; } = null!;
    
    public virtual TournamentThemeModel Theme { get; set; } = null!;
}