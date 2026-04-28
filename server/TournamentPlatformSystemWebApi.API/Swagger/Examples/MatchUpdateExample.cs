using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class MatchUpdateExample : IExamplesProvider<MatchUpdateDto>
    {
        public MatchUpdateDto GetExamples()
        {
            return new MatchUpdateDto
            {
                ScorePlayer1 = 2,
                ScorePlayer2 = 1,
                WinnerId = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-111213141516"),
                Status = "completed"
            };
        }
    }
}
