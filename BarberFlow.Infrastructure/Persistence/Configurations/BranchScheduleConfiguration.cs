using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations
{
    public class BranchScheduleConfiguration
        : IEntityTypeConfiguration<BranchSchedule>
    {
        public void Configure(
            EntityTypeBuilder<BranchSchedule> builder)
        {
            builder.ToTable("BranchSchedules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DayOfWeek)
                .IsRequired();

            builder.Property(x => x.OpenTime)
                .IsRequired();

            builder.Property(x => x.CloseTime)
                .IsRequired();

            builder.Property(x => x.IsClosed)
                .IsRequired();

            builder.HasIndex(x => new { x.BranchId, x.DayOfWeek })
                .IsUnique();
        }
    }
}
