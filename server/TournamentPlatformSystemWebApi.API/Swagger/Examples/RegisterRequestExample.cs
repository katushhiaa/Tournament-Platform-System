using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class RegisterRequestExample : IExamplesProvider<RegisterRequestDto>
    {
        public RegisterRequestDto GetExamples()
        {
            return new RegisterRequestDto
            {
                Email = "organizer@example.com",
                Password = "P@ssw0rd!",
                Name = "Olena Organizer",
                Role = "Organizer"
            };
        }
    }
}
