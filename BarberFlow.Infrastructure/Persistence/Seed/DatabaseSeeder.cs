namespace BarberFlow.Infrastructure.Persistence.Seed;
public static class DatabaseSeeder
{
    public static async Task SeedAsync(BarberFlowDbContext context)
    {
        await RoleSeeder.SeedAsync(context);
    }
}
