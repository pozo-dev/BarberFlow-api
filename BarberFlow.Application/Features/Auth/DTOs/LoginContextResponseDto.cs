namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class LoginContextResponseDto
    {
        public bool IsNewUser { get; set; }
        public Guid UserId { get; set; }
        public IReadOnlyCollection<LoginContextProfileDto> Profiles { get; set; } = [];
        public IReadOnlyCollection<int> EligibleNewProfileRoleIds { get; set; } = [];
    }
}
