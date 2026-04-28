using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentDtoExample : IExamplesProvider<TournamentDto>
    {
        public TournamentDto GetExamples()
        {
            return new TournamentDto
            {
                Id = Guid.Parse("d3b07384-d9a6-4a2a-9c2a-7b6c6a8e8f01"),
                Title = "Spring Championship 2026",
                StartDate = DateTime.Parse("2026-05-10T10:00:00Z").ToUniversalTime(),
                RegistrationCloseDate = DateTime.Parse("2026-05-01T23:59:59Z").ToUniversalTime(),
                Sport = Guid.NewGuid(),
                MaxParticipants = 32,
                Status = "draft"
            };
        }
    }
}
