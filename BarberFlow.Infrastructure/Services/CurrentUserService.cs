using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Auth.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BarberFlow.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                if (userId == null)
                    throw new UnauthorizedException();

                return Guid.Parse(userId);
            }
        }

        public Guid ProfileId
        {
            get
            {
                var profileId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("profileId")?
                    .Value;

                if (profileId == null)
                    throw new UnauthorizedException();

                return Guid.Parse(profileId);
            }
        }

        public int RoleId
        {
            get
            {
                var roleId = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("roleId")?
                    .Value;

                if (roleId == null)
                    throw new UnauthorizedException();

                return int.Parse(roleId);
            }
        }
    }
}
