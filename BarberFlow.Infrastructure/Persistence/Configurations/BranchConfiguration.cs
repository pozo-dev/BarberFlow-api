using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations
{
    public class BranchConfiguration
        : IEntityTypeConfiguration<Branch>
    {
        public void Configure(
            EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(x => x.Address)
                .HasMaxLength(300)
                .IsRequired();

            builder.HasOne(x => x.LocationSearch)
                .WithMany()
                .HasForeignKey(x => x.LocationSearchId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.TimeZoneId)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.IsMain)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasMany(x => x.Schedules)
                .WithOne(x => x.Branch)
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BarberAssignments)
                .WithOne(x => x.Branch)
                .HasForeignKey(x => x.BranchId);
        }
    }
}
