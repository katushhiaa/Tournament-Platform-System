using System;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using TournamentPlatformSystemWebApi.Application.DTOs.Auth;
using TournamentPlatformSystemWebApi.Application.Interfaces;
using TournamentPlatformSystemWebApi.Common.Models;
using TournamentPlatformSystemWebApi.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TournamentPlatformSystemWebApi.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [SwaggerTag("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Реєстрація нового користувача", Description = "Реєструє нового користувача. Роль: Guest.")]
        [SwaggerResponse(201, Type = typeof(RegisterUserResponse), Description = "Повертає токен та дані користувача")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Невалідні дані")]
        [SwaggerResponse(409, Type = typeof(ErrorResponseDto), Description = "Email вже існує")]
        [SwaggerRequestExample(typeof(RegisterUserRequest), typeof(Swagger.Examples.RegisterUserRequestExample))]
        [SwaggerResponseExample(201, typeof(Swagger.Examples.RegisterUserResponseExample))]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest dto)
        {
            try
            {
                var result = await _authenticationService.RegisterAsync(dto);
                return Created(string.Empty, result);
            }
            catch (ValidationException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 400,
                        Type = "ValidationError",
                        Message = ex.Message,
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return BadRequest(err);
            }
            catch (DuplicateEmailException ex)
            {
                var err = new ErrorResponseDto
                {
                    Error = new ErrorDetail
                    {
                        Code = 409,
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
                        Code = 500,
                        Type = "InternalServerError",
                        Message = "Internal server error",
                        Path = HttpContext.GetEndpoint()?.DisplayName,
                        Timestamp = DateTime.UtcNow.ToString("o"),
                        TraceId = HttpContext.TraceIdentifier
                    }
                };
                return StatusCode(500, err);
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Авторизація користувача", Description = "Авторизує користувача за email/password. Роль: Guest.")]
        [SwaggerResponse(200, Type = typeof(TokenResponseDto), Description = "Успішна авторизація")]
        [SwaggerResponse(400, Type = typeof(ErrorResponseDto), Description = "Помилка валідації")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Невірний логін/пароль")]
        [SwaggerRequestExample(typeof(LoginRequestDto), typeof(Swagger.Examples.LoginRequestExample))]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TokenResponseExample))]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            var user = new UserDto { Id = Guid.NewGuid(), Email = dto.Email, Name = "Sample User", Role = "Player" };
            var token = new TokenResponseDto { Token = "token-sample" };
            return Ok(new { token = token.Token, user });
        }

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Оновлення Access токена", Description = "Оновлює access токен з використанням refresh токена з cookie. Роль: Guest/Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(TokenResponseDto), Description = "Новий access токен")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Токен недійсний/прострочений")]
        [SwaggerResponseExample(200, typeof(Swagger.Examples.TokenResponseExample))]
        public IActionResult Refresh()
        {
            var token = new TokenResponseDto { Token = "new-token-sample" };
            return Ok(token);
        }

        [HttpGet("/api/v1/users/me")]
        [SwaggerOperation(Summary = "Отримання даних поточного користувача", Description = "Повертає профіль поточного користувача. Роль: Player/Organizer.")]
        [SwaggerResponse(200, Type = typeof(UserDto), Description = "Дані користувача")]
        [SwaggerResponse(401, Type = typeof(ErrorResponseDto), Description = "Токен відсутній або недійсний")]
        public IActionResult Me()
        {
            var user = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "player@example.com",
                Name = "Sample Player",
                Role = "Player",
                Stats = new { wins = 10, losses = 2 }
            };
            return Ok(user);
        }
    }
}

