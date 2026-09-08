using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Infrastructure.Security;
using BarberFlow.Infrastructure.Services;
using BarberFlow.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BarberFlow.Infrastructure.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        // JWT
        // Encryption
        // SMS
        // Email
        // File Storage
        // Cache

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenEncryptionService, TokenEncryptionService>();
        services.AddScoped<ISmsService, FakeSmsService>();

        return services;
    }
}
