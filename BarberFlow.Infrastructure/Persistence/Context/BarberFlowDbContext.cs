using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence
{
    public class BarberFlowDbContext : DbContext
    {
        public BarberFlowDbContext(DbContextOptions<BarberFlowDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<BarberShop> BarberShops { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentService> AppointmentServices { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BarberAssignment> BarberAssignments { get; set; }
        public DbSet<BranchSchedule> BranchSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberFlowDbContext).Assembly);
        }
    }
}
