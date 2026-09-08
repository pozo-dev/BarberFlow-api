using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

public sealed class CollaboratorWorkingHoursConfiguration : IEntityTypeConfiguration<CollaboratorWorkingHours>
{
    public void Configure(EntityTypeBuilder<CollaboratorWorkingHours> builder)
    {
        builder.ToTable("CollaboratorWorkingHours");
        builder.HasKey(x => x.CollaboratorId);
        builder.HasOne<Collaborator>().WithOne().HasForeignKey<CollaboratorWorkingHours>(x => x.CollaboratorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Periods).WithOne().HasForeignKey(x => x.CollaboratorId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CollaboratorWorkPeriodConfiguration : IEntityTypeConfiguration<CollaboratorWorkPeriod>
{
    public void Configure(EntityTypeBuilder<CollaboratorWorkPeriod> builder)
    {
        builder.ToTable("CollaboratorWorkPeriods", table =>
        {
            table.HasCheckConstraint("CK_CollaboratorWorkPeriods_Day", "[DayOfWeek] BETWEEN 1 AND 7");
            table.HasCheckConstraint("CK_CollaboratorWorkPeriods_Time", "[StartTime] < [EndTime]");
        });
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CollaboratorId, x.DayOfWeek });
    }
}

public sealed class CollaboratorTimeOffConfiguration : IEntityTypeConfiguration<CollaboratorTimeOff>
{
    public void Configure(EntityTypeBuilder<CollaboratorTimeOff> builder)
    {
        builder.ToTable("CollaboratorTimeOff", table =>
        {
            table.HasCheckConstraint("CK_CollaboratorTimeOff_Type", "[Type] IN (1, 2, 3)");
            table.HasCheckConstraint("CK_CollaboratorTimeOff_Status", "[Status] BETWEEN 1 AND 4");
            table.HasCheckConstraint("CK_CollaboratorTimeOff_Range", "[StartAtUtc] < [EndAtUtc]");
        });
        builder.HasKey(x => x.Id);
        builder.HasOne<Collaborator>().WithMany().HasForeignKey(x => x.CollaboratorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CollaboratorId, x.StartAtUtc, x.EndAtUtc });
        // Every domain instance explicitly chooses its state. The database must
        // not silently replace a requested Pending state with a default value.
        builder.Property(x => x.Status).ValueGeneratedNever();
    }
}
