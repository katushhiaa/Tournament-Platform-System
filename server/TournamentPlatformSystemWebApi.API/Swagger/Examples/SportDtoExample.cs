using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class SportDtoExample : IExamplesProvider<IEnumerable<SportDto>>
{
    public IEnumerable<SportDto> GetExamples()
    {
        return new List<SportDto>
        {
            new SportDto
            {
                Id = Guid.NewGuid(),
                Name = "Badminton"
            },
            new SportDto
            {
                Id = Guid.NewGuid(),
                Name = "Chess"
            }
        };
    }
}
