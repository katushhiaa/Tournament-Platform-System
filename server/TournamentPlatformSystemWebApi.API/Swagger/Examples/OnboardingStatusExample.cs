using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;

namespace TournamentPlatformSystemWebApi.API.Swagger.Examples;

public class OnboardingStatusExample : IExamplesProvider<OnboardingStatusDto>
{
    public OnboardingStatusDto GetExamples()
    {
        return new OnboardingStatusDto
        {
            PreferencesSetupCompleted = true
        };
    }
}
