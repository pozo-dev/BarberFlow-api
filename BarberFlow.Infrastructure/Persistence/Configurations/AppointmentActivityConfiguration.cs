using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;

public sealed class AppointmentActivityConfiguration
    : IEntityTypeConfiguration<AppointmentActivity>
{
    public void Configure(EntityTypeBuilder<AppointmentActivity> builder)
    {
        builder.ToTable("AppointmentActivities");

        builder.HasKey(activity => activity.Id);
        builder.Property(activity => activity.AppointmentId).IsRequired();
        builder.Property(activity => activity.Type).IsRequired();
        builder.Property(activity => activity.ActorProfileId).IsRequired();
        builder.Property(activity => activity.ActorRoleId).IsRequired();
        builder.Property(activity => activity.OccurredAtUtc).IsRequired();

        builder.HasIndex(activity => new
        {
            activity.AppointmentId,
            activity.OccurredAtUtc,
        });

        builder.HasOne(activity => activity.Appointment)
            .WithMany(appointment => appointment.Activities)
            .HasForeignKey(activity => activity.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
