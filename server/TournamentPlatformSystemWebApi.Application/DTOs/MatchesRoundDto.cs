using System.Collections.Generic;

namespace TournamentPlatformSystemWebApi.Application.DTOs
{
    public class MatchesRoundDto
    {
        public int Round { get; set; }
        public IReadOnlyList<MatchDto> Matches { get; set; } = new List<MatchDto>();
        public int MatchesCount { get; set; }
        public int NotByeMatchesCount { get; set; }
        public string? RoundDisplayName { get; set; }
    }
}
