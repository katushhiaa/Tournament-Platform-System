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
using TournamentPlatformSystemWebApi.Core.Entities;
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

        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Оновити турнір", Description = "Оновлює дані турніру. Роль: Organizer.")]
        [SwaggerResponse(200, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TournamentDto), Description = "Оновлено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Редагування заблоковано або конфлікт даних")]
        public async Task<IActionResult> UpdateTournament(Guid id, [FromBody] TournamentCreateDto dto)
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
                var updated = await _tournamentService.UpdateTournamentAsync(id, dto, organizerId);
                return Ok(updated);
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.DuplicateTournamentTitleException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentAlreadyStartedException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.InsufficientParticipantsException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException ex)
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

        [HttpGet]
        [SwaggerOperation(Summary = "Список турнірів (preview)", Description = "Повертає список турнірів для списків UI. Роль: Guest/Player/Organizer. Опційні параметри: page, pageSize, randomize, status (CSV; case-insensitive), is_personalized, q. Допустимі значення status: REGISTRATION_OPEN, REGISTRATION_CLOSED, IN_PROGRESS, COMPLETED, DRAFT.")]
        [SwaggerResponse(200, Type = typeof(TournamentPreviewListResponseDto), Description = "Список турнірів")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TournamentPreviewListResponseExample))]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        public async Task<IActionResult> GetAllTournaments([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool randomize = false, [FromQuery] string? status = null, [FromQuery(Name = "is_personalized")] bool isPersonalized = false, [FromQuery(Name = "q")] string? searchQuery = null)
        {
            try
            {
                var statuses = ParseStatuses(status);
                IReadOnlyList<TournamentPreviewDto> tournaments = Array.Empty<TournamentPreviewDto>();
                var isResponsePersonalized = false;
                string? fallbackReason = null;

                if (isPersonalized)
                {
                    var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
                    if (Guid.TryParse(sub, out var userId))
                    {
                        var preferredThemeIds = await _tournamentService.GetUserPreferredThemeIdsAsync(userId);

                        if (preferredThemeIds != null && preferredThemeIds.Count > 0)
                        {
                            tournaments = await _tournamentService.GetPersonalizedTournamentsAsync(preferredThemeIds, page, pageSize, randomize, statuses, searchQuery);
                            if (tournaments.Count > 0)
                            {
                                isResponsePersonalized = true;
                            }
                            else
                            {
                                fallbackReason = "no_matching_tournaments";
                            }
                        }
                        else
                        {
                            fallbackReason = "no_preferences";
                        }
                    }
                    else
                    {
                        fallbackReason = "not_authenticated";
                    }
                }

                if (!isResponsePersonalized)
                {
                    tournaments = await _tournamentService.GetAllTournamentsAsync(page, pageSize, randomize, statuses, searchQuery);
                }

                var response = new TournamentPreviewListResponseDto
                {
                    Tournaments = tournaments,
                    IsPersonalized = isResponsePersonalized,
                    FallbackReason = fallbackReason
                };

                return Ok(response);
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

        [HttpGet("user")]
        [SwaggerOperation(Summary = "Список турнірів користувача", Description = "Повертає список турнірів, де користувач є організатором або учасником. Роль: Guest/Player/Organizer. Опційні параметри: page, pageSize, status (CSV; case-insensitive). Допустимі значення status: REGISTRATION_OPEN, REGISTRATION_CLOSED, IN_PROGRESS, COMPLETED, DRAFT.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<TournamentPreviewDto>), Description = "Список турнірів користувача")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TournamentPreviewListExample))]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        public async Task<IActionResult> GetUserTournaments([FromQuery] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? status = null)
        {
            if (userId == Guid.Empty)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Message = "userId is required",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }

            try
            {
                var statuses = ParseStatuses(status);
                var tournaments = await _tournamentService.GetTournamentsForUserAsync(userId, page, pageSize, statuses);
                return Ok(tournaments);
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

        private static IReadOnlyList<TournamentStatus>? ParseStatuses(string? statusCsv)
        {
            if (string.IsNullOrWhiteSpace(statusCsv))
                return null;

            var tokens = statusCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (tokens.Length == 0)
                return null;

            var parsed = new List<TournamentStatus>();
            foreach (var token in tokens)
            {
                if (!Enum.TryParse<TournamentStatus>(token, true, out var status))
                    return null;

                parsed.Add(status);
            }

            return parsed.Distinct().ToList();
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

        [HttpPost("{id}/matches/{matchId}/result")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Зберегти результат матчу", Description = "Зберігає результат матчу. Роль: Organizer.")]
        [SwaggerResponse(200, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.MatchDto), Description = "Оновлений матч")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідний результат або матч не готовий")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Користувач не є організатором")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір або матч не знайдено")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Результат для цього матчу вже збережено")]
        [SwaggerRequestExample(typeof(TournamentPlatformSystemWebApi.Application.DTOs.MatchUpdateDto), typeof(Swagger.Examples.MatchUpdateExample))]
        public async Task<IActionResult> SaveMatchResult(Guid id, Guid matchId, [FromBody] TournamentPlatformSystemWebApi.Application.DTOs.MatchUpdateDto dto)
        {
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
                var updatedMatch = await _tournamentService.SaveMatchResultAsync(id, matchId, dto, organizerId);
                return Ok(updatedMatch);
            }
            catch (MatchNotFoundException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Message = "Match not found",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return NotFound(err);
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
            catch (MatchResultAlreadySavedException ex)
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
            catch (MatchNotReadyException ex)
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
        }

        [HttpGet("{id}/participants")]
        [SwaggerOperation(Summary = "Отримати учасників турніру", Description = "Повертає список учасників турніру.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<TournamentPlatformSystemWebApi.Application.DTOs.TeamDto>), Description = "Список учасників")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        public async Task<IActionResult> GetTournamentParticipants(Guid id)
        {
            try
            {
                var participants = await _tournamentService.GetTournamentParticipantsAsync(id);
                return Ok(participants);
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

        [HttpGet("{id}/events")]
        [SwaggerOperation(Summary = "Get tournament events", Description = "Returns timeline events for the tournament.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<TournamentPlatformSystemWebApi.Application.DTOs.EventDto>), Description = "List of events")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Tournament not found")]
        public async Task<IActionResult> GetTournamentEvents(Guid id)
        {
            try
            {
                var events = await _tournamentService.GetTournamentEventsAsync(id);
                return Ok(events);
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

        [HttpPost("{id}/participants")]
        [Authorize]
        [SwaggerOperation(Summary = "Додати учасника до турніру", Description = "Додає гравця до турніру. Приймає UserId гравця.")]
        [SwaggerResponse(201, Type = typeof(TournamentPlatformSystemWebApi.Application.DTOs.TeamDto), Description = "Учасник доданий")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Заборонено")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Користувач або турнір не знайдені")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Конфлікт: вже додано або досягнуто максимум учасників")]
        public async Task<IActionResult> AddParticipant(Guid id, [FromBody] TournamentPlatformSystemWebApi.Application.DTOs.AddParticipantRequestDto dto)
        {
            // get actor id from token (subject)
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var actorId))
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

            if (dto == null || dto.UserId == Guid.Empty)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Type = "BadRequest",
                        Message = "UserId is required",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return BadRequest(err);
            }

            try
            {
                var created = await _tournamentService.AddParticipantAsync(id, dto.UserId, actorId);

                return CreatedAtAction(nameof(GetTournamentDetails), new { id = id }, created);
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.ParticipantAlreadyAddedException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.MaxParticipantsReachedException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException ex)
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
            catch (KeyNotFoundException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };

                return NotFound(err);
            }
            catch (UnauthorizedAccessException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status403Forbidden,
                        Type = "Forbidden",
                        Message = ex.Message,
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

        [HttpDelete("{id}/participants")]
        [Authorize]
        [SwaggerOperation(Summary = "Вийти з турніру", Description = "Забирає поточного користувача з турніру, якщо реєстрація ще відкрита.")]
        [SwaggerResponse(204, Description = "Участь скасовано")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Реєстрація закрита або турнір вже почався/закінчений")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Forbidden")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір не знайдено")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Користувач не є учасником турніру")]
        public async Task<IActionResult> RemoveParticipant(Guid id)
        {
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var userId))
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
                await _tournamentService.RemoveParticipantAsync(id, userId);
                return NoContent();
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.ParticipantNotFoundException ex)
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
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException ex)
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

        [HttpDelete("{id}/participants/{userId}")]
        [Authorize(Roles = "organizer")]
        [SwaggerOperation(Summary = "Дискваліфікувати учасника", Description = "Дискваліфікує гравця з турніру. Роль: Organizer.")]
        [SwaggerResponse(204, Description = "Учасник дискваліфікований")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Forbidden")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Турнір або учасник не знайдені")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Турнір закритий для змін")]
        public async Task<IActionResult> DisqualifyParticipant(Guid id, Guid userId)
        {
            // actor id (organizer) from token
            var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var actorId))
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
                await _tournamentService.DisqualifyParticipantAsync(id, userId, actorId);
                return NoContent();
            }
            catch (TournamentPlatformSystemWebApi.Common.Exceptions.TournamentClosedForChangesException ex)
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
            catch (KeyNotFoundException)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = StatusCodes.Status404NotFound,
                        Type = "NotFound",
                        Message = "Tournament or participant not found",
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
    }
}
