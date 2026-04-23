using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class MatchUpdateDto
    {
        public int ScorePlayer1 { get; set; }
        public int ScorePlayer2 { get; set; }
        public Guid? WinnerId { get; set; }
        public string? Status { get; set; }
    }
}
