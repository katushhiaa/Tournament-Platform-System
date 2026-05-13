using System;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class TournamentStartResponse
    {
        public Guid TournamentId { get; set; }
        public string? Status { get; set; }
        public int MatchesCreated { get; set; }
    }
}
