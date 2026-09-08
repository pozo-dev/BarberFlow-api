using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class AppointmentServiceConfiguration : IEntityTypeConfiguration<AppointmentService>
{
    public void Configure(EntityTypeBuilder<AppointmentService> builder)
    {
        builder.ToTable("AppointmentServices");

        builder.HasKey(x => new { x.AppointmentId, x.ServiceId });

        builder.Property(x => x.PriceAtTheMoment)
            .IsRequired()
            .HasPrecision(10, 2);
    }
}
