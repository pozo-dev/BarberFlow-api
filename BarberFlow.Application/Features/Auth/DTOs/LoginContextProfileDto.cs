namespace BarberFlow.Application.Features.Auth.DTOs;
public class LoginContextProfileDto
{
    public Guid UserProfileId { get; set; }

    public int RoleId { get; set; }

    public string RoleName { get; set; } = string.Empty;
}
