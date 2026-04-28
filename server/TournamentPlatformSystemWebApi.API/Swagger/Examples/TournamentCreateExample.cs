using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentCreateExample : IExamplesProvider<TournamentCreateDto>
    {
        public TournamentCreateDto GetExamples()
        {
            return new TournamentCreateDto
            {
                Title = "Spring Championship 2026",
                StartDate = DateTime.Parse("2026-05-10T10:00:00Z").ToUniversalTime(),
                EndDate = DateTime.Parse("2026-05-12T10:00:00Z").ToUniversalTime(),
                RegistrationCloseDate = DateTime.Parse("2026-05-01T23:59:59Z").ToUniversalTime(),
                Sport = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-111213141516"),
                MaxParticipants = 32
            };
        }
    }
}
