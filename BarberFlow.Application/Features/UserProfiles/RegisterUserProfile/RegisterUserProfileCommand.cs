using MediatR;

namespace BarberFlow.Application.Features.UserProfiles.RegisterUserProfile
{
    public class RegisterUserProfileCommand : IRequest<RegisterUserProfileResponseDto>
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public Guid? BarberShopId { get; set; }
    }
}
