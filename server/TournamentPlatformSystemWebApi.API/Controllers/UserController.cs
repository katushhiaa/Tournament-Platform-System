using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs;
using TournamentPlatformSystemWebApi.Application.Interfaces;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [SwaggerTag("Users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService service)
        {
            _userService = service;
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Пошук користувачів", Description = "Повертає список користувачів за текстовим запитом. Якщо параметр q не задано — повертаємо перші 20 користувачів в алфавітному порядку по імені. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(IEnumerable<UserSearchItemResponce>), Description = "Колекція користувачів")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.UserSearchItemResponceExample))]
        public async Task<ActionResult<IReadOnlyList<UserSearchItemResponce>>> Get([FromQuery(Name = "q")] string q)
        {
            var users = await _userService.GetUsersWithQueryAsync(q);
            return Ok(users);
        }

    }
}