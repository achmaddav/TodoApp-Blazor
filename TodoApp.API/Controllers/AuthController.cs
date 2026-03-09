using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Auth.Commands.ChangePassword;
using TodoApp.Application.Interfaces;

namespace TodoApp.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public AuthController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;              
            _authService = authService;
        }


        /// <summary>Register new user</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto dto, CancellationToken ct)
        {
            var result = await _authService.RegisterAsync(dto, ct);
            return Ok(result);
        }

        /// <summary>Login and get JWT token</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto, CancellationToken ct)
        {
            var result = await _authService.LoginAsync(dto, ct);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDto dto, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new ChangePasswordCommand(dto.CurrentPassword, dto.NewPassword), ct);
            return HandleResult(result);
        }
    }
}
