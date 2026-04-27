using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Common.Models;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/matches")]
    [SwaggerTag("Matches")]
    public class MatchesController : ControllerBase
    {
        [HttpPatch("{matchId}")]
        [SwaggerOperation(Summary = "Оновлення результату матчу", Description = "Вносить результат матчу. Роль: Organizer.")]
        [SwaggerResponse(200, Type = typeof(MatchDto), Description = "Оновлено")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні результати")]
        [SwaggerResponse(403, Type = typeof(ErrorResponseDto), Description = "Не власник")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Матч не знайдено")]
        [SwaggerRequestExample(typeof(MatchUpdateDto), typeof(Swagger.Examples.MatchUpdateExample))]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.MatchDtoExample))]
        public IActionResult UpdateMatch(Guid matchId, [FromBody] MatchUpdateDto dto)
        {
            var updated = new MatchDto
            {
                MatchId = matchId,
                TournamentId = Guid.NewGuid(),
                Round = 1,
                Player1Id = Guid.NewGuid(),
                Player2Id = Guid.NewGuid(),
                Status = dto.Status ?? "completed",
                ScorePlayer1 = dto.ScorePlayer1,
                ScorePlayer2 = dto.ScorePlayer2
            };

            return Ok(updated);
        }

        [HttpGet("{tournamentId}")]
        [SwaggerOperation(Summary = "Список матчів", Description = "Повертає список матчів за заданим id турніра. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(MatchDto), Description = "Колекція матчів")]
        [SwaggerResponse(404, Type = typeof(ErrorResponseDto), Description = "Матчі турніру не знайдено")]
        public IActionResult GetMatches(Guid tournamentId)
        {
            var sample = new List<MatchDto> {
                new MatchDto
                {
                    MatchId = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Round = 2,
                    OrderNumber = 4,
                    Player1Id = Guid.NewGuid(),
                    Player2Id = null,
                    Status = "pending",
                    ScorePlayer1 = null,
                    ScorePlayer2 = null
                },
                new MatchDto
                {
                    MatchId = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Round = 2,
                    OrderNumber = 4,
                    Player1Id = Guid.NewGuid(),
                    Player2Id = Guid.NewGuid(),
                    Status = "Completed",
                    ScorePlayer1 = 2,
                    ScorePlayer2 = 1
                },
            };
            return Ok(sample);
        }
    }
}
