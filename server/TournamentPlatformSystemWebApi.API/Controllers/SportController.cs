using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [Route("api/v1/sport")]
    [ApiController]
    [SwaggerTag("Sport")]
    public class SportController : ControllerBase
    {
        private readonly IThemeRepository _repository;

        public SportController(IThemeRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Список доступних видів спорту", Description = "Повертає список видів спорту. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<SportDto>), Description = "Колекція видів спорту")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.SportDtoExample))]
        public async Task<IActionResult> GetSports()
        {
            var sports = await _repository.GetAllAsync();

            return Ok(sports.Select(s => new SportDto
            {
                Name = s.Name,
                Id = s.Id
            }));
        }
    }
}
