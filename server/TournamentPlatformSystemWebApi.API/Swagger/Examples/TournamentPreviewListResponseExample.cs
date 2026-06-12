using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentPreviewListResponseExample : IExamplesProvider<TournamentPreviewListResponseDto>
    {
        public TournamentPreviewListResponseDto GetExamples()
        {
            return new TournamentPreviewListResponseDto
            {
                Tournaments = new List<TournamentPreviewDto>
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
                    }
                },
                IsPersonalized = true,
                FallbackReason = null
            };
        }
    }
}
