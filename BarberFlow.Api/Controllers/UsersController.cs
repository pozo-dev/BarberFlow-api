using BarberFlow.Application.Features.UserProfiles.RegisterUserProfile;
using BarberFlow.Application.Features.Users.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            var id = await _mediator.Send(command);

            return Ok(id);
        }

        [HttpPost("profiles")]
        [Authorize]
        public async Task<ActionResult<RegisterUserProfileResponseDto>> RegisterProfile(
        RegisterUserProfileCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
