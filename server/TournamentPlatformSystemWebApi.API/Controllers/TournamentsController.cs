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
                return BadRequest(new ErrorResponseDto { Error = new ErrorDetail { Message = ex.Message, Code = 400 } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = new ErrorDetail { Message = ex.Message, Code = 500 } });
            }
        }

        [HttpPost("{id}/image")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Upload tournament image", Description = "Uploads image to configured storage and attaches to tournament. Role: Organizer.")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
        {
            if (file == null)
                return BadRequest(new ErrorResponseDto { Error = new ErrorDetail { Message = "File is required", Code = 400 } });

            // Check size and content type
            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
                return BadRequest(new ErrorResponseDto { Error = new ErrorDetail { Message = "File is empty or too large (max 5MB)", Code = 400 } });
            if (Array.IndexOf(allowed, file.ContentType) < 0)
                return BadRequest(new ErrorResponseDto { Error = new ErrorDetail { Message = "Unsupported file type", Code = 400 } });

            // organizer id from token
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var organizerId))
            {
                return Unauthorized(new ErrorResponseDto { Error = new ErrorDetail { Message = "Invalid user", Code = 401 } });
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
                return BadRequest(new ErrorResponseDto { Error = new ErrorDetail { Message = ex.Message, Code = 400 } });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ErrorResponseDto { Error = new ErrorDetail { Message = "Tournament not found", Code = 404 } });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto { Error = new ErrorDetail { Message = ex.Message, Code = 500 } });
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
                Sport = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-111213141516"),
                MaxParticipants = 16,
                Status = "active"
            };
            return Ok(sample);
        }
    }
}
