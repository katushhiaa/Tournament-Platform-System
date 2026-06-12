using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/users/me")]
    [SwaggerTag("User Preferences")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IUserPreferencesService _preferencesService;

        public UserPreferencesController(IUserPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        [HttpGet("onboarding-status")]
        [SwaggerOperation(Summary = "Статус проходження онбордингу", Description = "Повертає статус завершення кроку вибору видів спорту. Роль: Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(OnboardingStatusDto), Description = "Статус онбордингу")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.OnboardingStatusExample))]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Неавторизовано")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Користувач не знайдений")]
        public async Task<IActionResult> GetOnboardingStatus()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(BuildUnauthorizedError());
            }

            try
            {
                var completed = await _preferencesService.GetPreferencesSetupCompletedAsync(userId);
                return Ok(new OnboardingStatusDto { PreferencesSetupCompleted = completed });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BuildNotFoundError(ex.Message));
            }
        }

        [HttpPatch("onboarding-complete")]
        [SwaggerOperation(Summary = "Позначити онбординг завершеним. ЦЕ ТИПУ SKIP. ", Description = "Встановлює preferences_setup_completed = true. Роль: Player/Organizer.")]
        [SwaggerResponse(204, Description = "Оновлено")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Неавторизовано")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Користувач не знайдений")]
        public async Task<IActionResult> CompleteOnboarding()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(BuildUnauthorizedError());
            }

            try
            {
                await _preferencesService.MarkPreferencesSetupCompletedAsync(userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BuildNotFoundError(ex.Message));
            }
        }

        [HttpPut("preferences")]
        [SwaggerOperation(Summary = "Оновити преференції користувача", Description = "Зберігає список улюблених видів спорту. Роль: Player/Organizer.")]
        [SwaggerRequestExample(typeof(UserPreferencesUpdateRequest), typeof(Swagger.Examples.UserPreferencesUpdateRequestExample))]
        [SwaggerResponse(204, Description = "Оновлено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Неавторизовано")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Користувач не знайдений")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferencesUpdateRequest request)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized(BuildUnauthorizedError());
            }

            try
            {
                await _preferencesService.UpdateUserThemePreferencesAsync(userId, request.ThemeIds);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(BuildValidationError(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(BuildNotFoundError(ex.Message));
            }
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out userId);
        }

        private ErrorResponseDto BuildUnauthorizedError()
        {
            return new ErrorResponseDto
            {
                Error = new ErrorDetail
                {
                    Code = StatusCodes.Status401Unauthorized,
                    Type = "Unauthorized",
                    Message = "Invalid or missing access token",
                    Path = HttpContext.Request.Path,
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TraceId = HttpContext.TraceIdentifier
                }
            };
        }

        private ErrorResponseDto BuildNotFoundError(string message)
        {
            return new ErrorResponseDto
            {
                Error = new ErrorDetail
                {
                    Code = StatusCodes.Status404NotFound,
                    Type = "NotFound",
                    Message = message,
                    Path = HttpContext.Request.Path,
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TraceId = HttpContext.TraceIdentifier
                }
            };
        }

        private ErrorResponseDto BuildValidationError(string message)
        {
            return new ErrorResponseDto
            {
                Error = new ErrorDetail
                {
                    Code = StatusCodes.Status400BadRequest,
                    Type = "ValidationError",
                    Message = message,
                    Path = HttpContext.Request.Path,
                    Timestamp = DateTime.UtcNow.ToString("o"),
                    TraceId = HttpContext.TraceIdentifier
                }
            };
        }
    }
}
