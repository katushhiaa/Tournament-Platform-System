using System;

namespace TournamentPlatformSystem.Application.DTOs
{
    public class MatchDto
    {
        public Guid MatchId { get; set; }
        public Guid TournamentId { get; set; }
        public int Round { get; set; }
        public int OrderNumber { get; set; }
        public Guid? Player1Id { get; set; }
        public Guid? Player2Id { get; set; }
        public string? Status { get; set; }
        public int? ScorePlayer1 { get; set; }
        public int? ScorePlayer2 { get; set; }
        public Guid? WinnerId { get; set; }

    }
}
