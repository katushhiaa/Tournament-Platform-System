using System;

namespace TournamentPlatformSystemWebApi.Core.Entities
{
    public class UserTeam
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? TeamName { get; set; }
        public string? UserName { get; set; }
    }
}
