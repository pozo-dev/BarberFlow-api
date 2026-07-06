using BarberFlow.Domain.Entities;

namespace BarberFlow.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, UserProfile userProfile);
        string GenerateRefreshToken();
        DateTime GetAccessTokenExpiration();
    }
}
