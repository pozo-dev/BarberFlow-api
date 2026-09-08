using MediatR;

namespace BarberFlow.Application.Features.UserProfiles.RegisterUserProfile;
public class RegisterUserProfileCommand : IRequest<RegisterUserProfileResponseDto>
{
    public Guid UserId { get; set; }

    public int RoleId { get; set; }

    public Guid? BarberShopId { get; set; }
}
