using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystem.Application.DTOs;
using TournamentPlatformSystemWebApi.Common.Models;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/tournaments")]
    [SwaggerTag("Tournaments")]
    public class TournamentsController : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Створення нового турніру", Description = "Створює новий турнір. Роль: Organizer.")]
        [SwaggerResponse(201, Type = typeof(TournamentDto), Description = "Створено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Не авторизований")]
        [SwaggerRequestExample(typeof(TournamentCreateDto), typeof(Swagger.Examples.TournamentCreateExample))]
        [SwaggerResponseExample(201, typeof(Swagger.Examples.TournamentDtoExample))]
        public IActionResult CreateTournament([FromBody] TournamentCreateDto dto)
        {
            var created = new TournamentDto
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                StartDate = dto.StartDate,
                RegistrationCloseDate = dto.RegistrationCloseDate,
                Sport = dto.Sport,
                MaxParticipants = dto.MaxParticipants,
                Status = "draft"
            };

            return CreatedAtAction(nameof(GetTournament), new { id = created.Id }, created);
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
