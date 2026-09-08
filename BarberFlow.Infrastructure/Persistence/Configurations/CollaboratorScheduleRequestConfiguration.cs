using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

public sealed class CollaboratorScheduleRequestConfiguration : IEntityTypeConfiguration<CollaboratorScheduleRequest>
{
    public void Configure(EntityTypeBuilder<CollaboratorScheduleRequest> builder)
    {
        builder.ToTable("CollaboratorScheduleRequests", t => t.HasCheckConstraint("CK_ScheduleRequest_Status", "[Status] BETWEEN 1 AND 4"));
        builder.HasKey(x => x.Id);
        builder.HasOne<Collaborator>().WithMany().HasForeignKey(x => x.CollaboratorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.CollaboratorId).IsUnique().HasFilter("[Status] = 1");
        builder.HasMany(x => x.Periods).WithOne().HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CollaboratorRequestedPeriodConfiguration : IEntityTypeConfiguration<CollaboratorRequestedPeriod>
{
    public void Configure(EntityTypeBuilder<CollaboratorRequestedPeriod> builder)
    {
        builder.ToTable("CollaboratorRequestedPeriods", t =>
        {
            t.HasCheckConstraint("CK_RequestedPeriod_Day", "[DayOfWeek] BETWEEN 1 AND 7");
            t.HasCheckConstraint("CK_RequestedPeriod_Time", "[StartTime] < [EndTime]");
        });
        builder.HasKey(x => x.Id);
    }
}
