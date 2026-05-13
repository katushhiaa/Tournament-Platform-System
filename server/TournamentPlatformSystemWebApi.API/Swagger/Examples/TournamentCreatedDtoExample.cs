using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class TournamentCreatedDtoExample : IExamplesProvider<TournamentCreatedDto>
    {
        public TournamentCreatedDto GetExamples()
        {
            return new TournamentCreatedDto
            {
                Id = Guid.Parse("e7b1c3d2-4f5a-4e6b-9a1b-2c3d4e5f6789"),
                DetailsUrl = "https://api.example.com/api/v1/tournaments/e7b1c3d2-4f5a-4e6b-9a1b-2c3d4e5f6789/details"
            };
        }
    }
}
