using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class AppointmentConfiguration
    : IEntityTypeConfiguration<Appointment>
{
    public void Configure(
        EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.BranchId)
            .IsRequired();

        builder.Property(x => x.CollaboratorId)
            .IsRequired();

        builder.Property(x => x.StartDateTime)
            .IsRequired();

        builder.Property(x => x.EndDateTime)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.RescheduledToAppointmentId);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Appointment -> Branch
        builder
            .HasOne(x => x.Branch)
            .WithMany(x => x.Appointments)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Collaborator)
            .WithMany()
            .HasForeignKey(x => x.CollaboratorId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BranchId, x.CollaboratorId, x.StartDateTime });

        // Appointment -> AppointmentServices
        builder
            .HasMany(x => x.AppointmentServices)
            .WithOne(x => x.Appointment)
            .HasForeignKey(x => x.AppointmentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
