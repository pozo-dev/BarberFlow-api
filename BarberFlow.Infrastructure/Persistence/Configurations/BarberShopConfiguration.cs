using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations
{
    public class BarberShopConfiguration : IEntityTypeConfiguration<BarberShop>
    {
        public void Configure(EntityTypeBuilder<BarberShop> builder)
        {
            builder.ToTable("BarberShops");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.OwnerUserId)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasMany(x => x.Services)
                .WithOne(x => x.BarberShop)
                .HasForeignKey(x => x.BarberShopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Appointments)
                .WithOne(x => x.BarberShop)
                .HasForeignKey(x => x.BarberShopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Barbers)
                .WithOne(x => x.BarberShop)
                .HasForeignKey(x => x.BarberShopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
