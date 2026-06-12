using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class MatchesRoundListExample : IExamplesProvider<IEnumerable<MatchesRoundDto>>
    {
        public IEnumerable<MatchesRoundDto> GetExamples()
        {
            var m1 = new MatchDto
            {
                MatchId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                TournamentId = Guid.Parse("e9431297-0648-4ddb-9d1a-37fddbeb9120"),
                Round = 1,
                OrderNumber = 1,
                Player1Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Player2Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Status = "completed",
                IsBye = false,
                ScorePlayer1 = 2,
                ScorePlayer2 = 1,
                WinnerId = Guid.Parse("11111111-1111-1111-1111-111111111111")
            };

            var m2 = new MatchDto
            {
                MatchId = Guid.Parse("a47ac10b-58cc-4372-a567-0e02b2c3d470"),
                TournamentId = Guid.Parse("e9431297-0648-4ddb-9d1a-37fddbeb9120"),
                Round = 1,
                OrderNumber = 2,
                Player1Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Player2Id = null,
                Status = "bye",
                IsBye = true,
                ScorePlayer1 = null,
                ScorePlayer2 = null,
                WinnerId = Guid.Parse("33333333-3333-3333-3333-333333333333")
            };

            var final = new MatchDto
            {
                MatchId = Guid.Parse("b47ac10b-58cc-4372-a567-0e02b2c3d471"),
                TournamentId = Guid.Parse("e9431297-0648-4ddb-9d1a-37fddbeb9120"),
                Round = 4,
                OrderNumber = 1,
                Player1Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Player2Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Status = "scheduled",
                IsBye = false,
                ScorePlayer1 = null,
                ScorePlayer2 = null,
                WinnerId = null
            };

            var round1 = new MatchesRoundDto
            {
                Round = 1,
                Matches = new List<MatchDto> { m1, m2 },
                MatchesCount = 2,
                NotByeMatchesCount = 1,
                RoundDisplayName = "1/8"
            };

            var roundFinal = new MatchesRoundDto
            {
                Round = 4,
                Matches = new List<MatchDto> { final },
                MatchesCount = 1,
                NotByeMatchesCount = 1,
                RoundDisplayName = "final"
            };

            return new[] { round1, roundFinal };
        }
    }
}
