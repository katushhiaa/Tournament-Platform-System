using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class MatchDtoExample : IExamplesProvider<MatchDto>
    {
        public MatchDto GetExamples()
        {
            return new MatchDto
            {
                MatchId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                TournamentId = Guid.Parse("d3b07384-d9a6-4a2a-9c2a-7b6c6a8e8f01"),
                Round = 1,
                Player1Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Player2Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Status = "completed",
                ScorePlayer1 = 2,
                ScorePlayer2 = 1,
                WinnerId = Guid.Parse("22222222-2222-2222-2222-222222222222")
            };
        }
    }
}
