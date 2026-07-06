namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class LoginContextResponseDto
    {
        //public bool UserExists { get; set; }

        //public bool RequiresProfileSelection { get; set; }

        public bool IsNewUser { get; set; }
        public IReadOnlyCollection<LoginContextProfileDto> Profiles { get; set; } = [];
    }
}
