using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentPreviewListExample : IExamplesProvider<IEnumerable<TournamentPreviewDto>>
    {
        public IEnumerable<TournamentPreviewDto> GetExamples()
        {
            return new List<TournamentPreviewDto>
            {
                new TournamentPreviewDto
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Spring Open",
                    Status = "registration_open",
                    BackgroundImg = "https://example.com/images/spring-open.jpg",
                    SportName = "chess",
                    StartDate = DateTime.Parse("2026-06-01T10:00:00Z").ToUniversalTime(),
                    ParticipantsCount = 8,
                    MaxParticipants = 32
                },
                new TournamentPreviewDto
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "Summer Qualifier",
                    Status = "registration_closed",
                    BackgroundImg = "https://example.com/images/summer-qualifier.jpg",
                    SportName = "tennis",
                    StartDate = DateTime.Parse("2026-06-10T12:00:00Z").ToUniversalTime(),
                    ParticipantsCount = 16,
                    MaxParticipants = 16
                },
                new TournamentPreviewDto
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Title = "Midseason Cup",
                    Status = "in_progress",
                    BackgroundImg = "https://example.com/images/midseason-cup.jpg",
                    SportName = "boxing",
                    StartDate = DateTime.Parse("2026-06-15T09:00:00Z").ToUniversalTime(),
                    ParticipantsCount = 12,
                    MaxParticipants = 16
                },
                new TournamentPreviewDto
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Title = "Finals Weekend",
                    Status = "completed",
                    BackgroundImg = "https://example.com/images/finals-weekend.jpg",
                    SportName = "shooting",
                    StartDate = DateTime.Parse("2026-05-20T15:00:00Z").ToUniversalTime(),
                    ParticipantsCount = 8,
                    MaxParticipants = 8
                },
                new TournamentPreviewDto
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Title = "Draft Tournament",
                    Status = "draft",
                    BackgroundImg = "https://example.com/images/draft-tournament.jpg",
                    SportName = "rocket league",
                    StartDate = DateTime.Parse("2026-07-01T18:00:00Z").ToUniversalTime(),
                    ParticipantsCount = 0,
                    MaxParticipants = 32
                }
            };
        }
    }
}
