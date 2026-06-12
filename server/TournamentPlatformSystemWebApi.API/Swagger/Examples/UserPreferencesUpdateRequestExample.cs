using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class UserPreferencesUpdateRequestExample : IExamplesProvider<UserPreferencesUpdateRequest>
{
    public UserPreferencesUpdateRequest GetExamples()
    {
        return new UserPreferencesUpdateRequest
        {
            ThemeIds = new List<Guid>
            {
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222")
            }
        };
    }
}
