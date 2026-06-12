using System;

namespace TournamentPlatformSystemWebApi.Infrastructure.Entities;

public partial class RefreshTokenModel : BaseDbEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string JwtId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
