using BarberFlow.Application.Features.Auth.Commands.LoginWithOtp;
using BarberFlow.Application.Features.Auth.Commands.Logout;
using BarberFlow.Application.Features.Auth.Commands.RefreshUserToken;
using BarberFlow.Application.Features.Auth.Commands.VerifyOtp;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Application.Features.Auth.Queries.LoginContext;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login-context")]
        public async Task<ActionResult<LoginContextResponseDto>> GetLoginContext(GetLoginContextQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [EnableRateLimiting("OtpPolicy")]
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (Exception)
            {
                // Mensaje genérico para evitar filtrado de información
                return BadRequest(new { message = "No se pudo generar el código OTP." });
            }
        }

        [EnableRateLimiting("OtpPolicy")]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Devuelve siempre un mensaje genérico para evitar enumeración de usuarios
                return Unauthorized(new { message = "Credenciales inválidas." });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}

