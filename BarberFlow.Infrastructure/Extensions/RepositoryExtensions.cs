using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Infrastructure.Persistence;
using BarberFlow.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BarberFlow.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IBarberShopRepository, BarberShopRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IServicePriceRepository, ServicePriceRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IBarberAssignmentRepository, BarberAssignmentRepository>();

            return services;
        }
    }
}
