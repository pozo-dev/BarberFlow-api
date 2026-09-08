using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class BarberAssignmentConfiguration
    : IEntityTypeConfiguration<BarberAssignment>
{
    public void Configure(EntityTypeBuilder<BarberAssignment> builder)
    {
        builder.ToTable("BarberAssignments");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.BranchId, x.BarberProfileId })
            .IsUnique();

        builder.HasOne(x => x.Branch)
            .WithMany(x => x.BarberAssignments)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.BarberProfile)
            .WithMany(x => x.BarberAssignments)
            .HasForeignKey(x => x.BarberProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
