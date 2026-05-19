using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class UserSearchItemResponceExample : IExamplesProvider<IEnumerable<UserSearchItemResponce>>
{
    public IEnumerable<UserSearchItemResponce> GetExamples()
    {
        return new List<UserSearchItemResponce>
        {
            new UserSearchItemResponce { Id = Guid.NewGuid(), FullName = "Ivan Petrenko" },
            new UserSearchItemResponce { Id = Guid.NewGuid(), FullName = "Olena Shevchenko" }
        };
    }
}
