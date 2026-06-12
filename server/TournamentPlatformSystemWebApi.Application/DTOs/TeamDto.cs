using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
        public Guid TournamentId { get; set; }
        public bool? IsDisqualified { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
