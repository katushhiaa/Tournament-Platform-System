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
        [SwaggerResponse(201, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TournamentCreatedDto), Description = "Створено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Forbidden")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Турнір з такою назвою вже створений цим організатором")]
        [SwaggerRequestExample(typeof(TournamentCreateDto), typeof(Swagger.Examples.TournamentCreateExample))]
        [SwaggerResponseExample(201, typeof(Swagger.Examples.TournamentCreatedDtoExample))]
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

                var detailsUrl = Url.Action(nameof(GetTournamentDetails), "Tournaments", new { id = created.Id }, Request.Scheme) ?? $"/api/v1/tournaments/{created.Id}/details";

                var response = new TournamentPlatformSystemWebApi.Application.DTOs.TournamentCreatedDto
                {
                    Id = created.Id,
                    DetailsUrl = detailsUrl
                };

                return CreatedAtAction(nameof(GetTournamentDetails), new { id = created.Id }, response);
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

        [HttpPost("draft")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Зберегти чернетку турніру", Description = "Зберігає турнір як чернетку. Роль: Organizer.")]
        [SwaggerResponse(201, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TournamentCreatedDto), Description = "Чернетка збережена")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        public async Task<IActionResult> SaveTournamentDraft([FromBody] TournamentCreateDto dto)
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
                var created = await _tournamentService.SaveTournamentDraftAsync(dto, organizerId);

                var detailsUrl = Url.Action(nameof(GetTournamentDetails), "Tournaments", new { id = created.Id }, Request.Scheme) ?? $"/api/v1/tournaments/{created.Id}/details";

                var response = new TournamentPlatformSystemWebApi.Application.DTOs.TournamentCreatedDto
                {
                    Id = created.Id,
                    DetailsUrl = detailsUrl
                };

                return CreatedAtAction(nameof(GetTournamentDetails), new { id = created.Id }, response);
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
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Запуск турніру", Description = "Запускає турнір та генерує турнірну сітку. Роль: Organizer.")]
        [SwaggerResponse(200, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TournamentStartResponse), Description = "Турнір запущено; повертає мінімальну інформацію щодо запуску")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TournamentStartResponseExample))]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Не власник")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Недостатньо учасників або вже активний")]
        public async Task<IActionResult> StartTournament(Guid id)
        {
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

            try
            {
                var dto = await _tournamentService.StartTournament(id, organizerId);

                return Ok(dto);
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
                        Message = "Not the organizer",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return StatusCode(StatusCodes.Status403Forbidden, err);
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentAlreadyStartedException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status409Conflict,
                        Type = "Conflict",
                        Message = "Tournament already started",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return Conflict(err);
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.InsufficientParticipantsException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status409Conflict,
                        Type = "Conflict",
                        Message = "Not enough participants to start the tournament",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return Conflict(err);
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


        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Одержати повні деталі турніру", Description = "Повертає повну інформацію про турнір: учасники, матчі, метадані. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TournamentDetailsDto), Description = "Повні деталі турніру")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        public async Task<IActionResult> GetTournamentDetails(Guid id)
        {
            try
            {
                var tournamentDto = await _tournamentService.GetTournamentDetailsAsync(id);

                if (tournamentDto.Status == "draft")
                {
                    var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                    if (!Guid.TryParse(sub, out var userId) || userId != tournamentDto.OrganizerId)
                    {
                        var err = new ErrorResponseDto
                        {
                            Error = new ErrorDetail
                            {
                                Code = StatusCodes.Status403Forbidden,
                                Type = "Forbidden",
                                Message = "Tournament is in draft and access is restricted",
                                Path = HttpContext.GetEndpoint()?.DisplayName,
                                Timestamp = DateTime.UtcNow.ToString("o"),
                                TraceId = HttpContext.TraceIdentifier
                            }
                        };

                        return StatusCode(StatusCodes.Status403Forbidden, err);
                    }
                }

                return Ok(tournamentDto);
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
        }

        [HttpGet("{id}/matches")]
        [SwaggerOperation(Summary = "Одержати матчі турніру", Description = "Повертає матчі, згруповані по раундах.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<TournamentPlatformSystemWebApi.Application.DTOs.MatchesRoundDto>), Description = "Список раундів з матчами")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.MatchesRoundListExample))]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        public async Task<IActionResult> GetTournamentMatches(Guid id)
        {
            try
            {
                var rounds = await _tournamentService.GetTournamentMatchesAsync(id);
                return Ok(rounds);
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
        }
    }
}
