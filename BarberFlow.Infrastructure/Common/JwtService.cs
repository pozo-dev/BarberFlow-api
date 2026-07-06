using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Entities;
using BarberFlow.Infrastructure.Common.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    
    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateAccessToken(User user, UserProfile userProfile)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
            new Claim("profileId", userProfile.Id.ToString()),
            new Claim("roleId", userProfile.RoleId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

        //var x = string.Empty;
        //try
        //{
        //    x = new JwtSecurityTokenHandler().WriteToken(token);

        //} catch(Exception ex)
        //{

        //}
        //return x;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }

    public DateTime GetAccessTokenExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);
    }
}