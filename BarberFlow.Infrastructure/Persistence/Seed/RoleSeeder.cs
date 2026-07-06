using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(BarberFlowDbContext context)
        {
            if (await context.Roles.AnyAsync())
                return;

            context.Roles.AddRange(
                new Role(RoleIds.Client,RoleNames.Client),
                new Role(RoleIds.Barber, RoleNames.Barber),
                new Role(RoleIds.Owner, RoleNames.Owner),
                new Role(RoleIds.Admin, RoleNames.Admin)
            );

            await context.SaveChangesAsync();
        }
    }
}
