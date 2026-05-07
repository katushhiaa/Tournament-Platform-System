using System;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples
{
    public class RegisterUserRequestExample : IExamplesProvider<RegisterUserRequest>
    {
        public RegisterUserRequest GetExamples()
        {
            return new RegisterUserRequest
            {
                Email = "organizer@example.com",
                Password = "P@ssw0rd!",
                FullName = "Olena Organizer",
                Role = "organizer",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-20),
                PhoneNumber = "+380661234567"
            };
        }
    }
}
