using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Common.Models;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TournamentPlatformSystemWebApi.Common.Exceptions;
namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/tournaments")]
    [SwaggerTag("Tournaments")]

    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        public TournamentsController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }
        [HttpPost]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Створення нового турніру", Description = "Створює новий турнір. Роль: Organizer.")]
        [SwaggerResponse(201, Type = typeof(TournamentDto), Description = "Створено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Forbidden")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Турнір з такою назвою вже створений цим організатором")]
        [SwaggerRequestExample(typeof(TournamentCreateDto), typeof(Swagger.Examples.TournamentCreateExample))]
        [SwaggerResponseExample(201, typeof(Swagger.Examples.TournamentDtoExample))]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentCreateDto dto)
        {

            // get organizer id from token (subject)
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var organizerId))
            {
                return Unauthorized(new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Type = "Unauthorized",
                        Message = "Invalid user",
                        Code = 401,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                });
            }
            try
            {
                var created = await _tournamentService.CreateTournamentAsync(dto, organizerId);

                return CreatedAtAction(nameof(GetTournament), new { id = created.Id }, created);
            }
            catch (ValidationException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "ValidationError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }
            catch (DuplicateTournamentTitleException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status409Conflict,
                        Type = "Conflict",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return Conflict(err);
            }
            catch (ArgumentException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "ArgumentError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Type = "InternalServerError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return StatusCode(500, err);
            }
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Upload tournament image", Description = "Uploads image to configured storage and attaches to tournament. Role: Organizer.")]
        [SwaggerResponse(200, Type = typeof(object), Description = "Upload successful; returns URL and file id")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "File is required, invalid or too large")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Unauthorized")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Forbidden")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Tournament not found")]
        [SwaggerResponse(500, Type = typeof(ErrorResponseDto), Description = "Internal server error")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file == null)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Message = "File is required",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }

            // Check size and content type
            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Message = "File is empty or too large (max 5MB)",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }
            if (Array.IndexOf(allowed, file.ContentType) < 0)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Message = "Unsupported file type",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }

            // organizer id from token
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var organizerId))
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status401Unauthorized,
                        Type = "Unauthorized",
                        Message = "Invalid user",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return Unauthorized(err);
            }

            // Copy to memory stream and delegate upload to service
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            try
            {
                var upload = await _tournamentService.UploadImageAsync(id, organizerId, ms, file.FileName, file.ContentType, file.Length);
                return Ok(new { url = upload.Url, id = upload.FileId });
            }
            catch (ValidationException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "ValidationError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }
            catch (KeyNotFoundException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Message = "Tournament not found",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return NotFound(err);
            }
            catch (UnauthorizedAccessException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status403Forbidden,
                        Type = "Forbidden",
                        Message = "Forbidden",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return StatusCode(StatusCodes.Status403Forbidden, err);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Type = "InternalServerError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return StatusCode(500, err);
            }
        }

        [HttpPost("{id}/start")]
        [SwaggerOperation(Summary = "Запуск турніру", Description = "Запускає турнір та генерує турнірну сітку. Роль: Organizer.")]
        [SwaggerResponse(200, Type = typeof(object), Description = "Турнір запущено")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Не власник")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Недостатньо учасників або вже активний")]
        public IActionResult StartTournament(Guid id)
        {
            return Ok(new { message = "Tournament started", tournamentId = id });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Одержати деталі турніру", Description = "Повертає деталі турніру. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(TournamentDto), Description = "Деталі турніру")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        public IActionResult GetTournament(Guid id)
        {
            var sample = new TournamentDto
            {
                Id = id,
                Title = "Sample Tournament",
                StartDate = DateTime.UtcNow.AddDays(14),
                EndDate = DateTime.UtcNow.AddDays(16),
                RegistrationCloseDate = DateTime.UtcNow.AddDays(7),
                SportId = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-111213141516"),
                MaxParticipants = 16,
                Status = "active"
            };
            return Ok(sample);
        }
    }
}
