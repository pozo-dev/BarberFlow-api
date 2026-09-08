using BarberFlow.Application.Features.UserProfiles.RegisterUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers;
[ApiController]
[Route("api/auth/user-profiles")]
[AllowAnonymous]
public class UserProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterUserProfileResponseDto>> Create(
        RegisterUserProfileCommand command)
    {
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}
