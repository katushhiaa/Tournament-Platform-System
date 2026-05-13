using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentStartResponseExample : IExamplesProvider<TournamentStartResponse>
    {
        public TournamentStartResponse GetExamples()
        {
            return new TournamentStartResponse
            {
                TournamentId = Guid.Parse("e9431297-0648-4ddb-9d1a-37fddbeb9120"),
                Status = "in_progress",
                MatchesCreated = 15
            };
        }
    }
}
