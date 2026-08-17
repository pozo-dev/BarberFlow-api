using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations
{
    public class LocationSearchConfiguration : IEntityTypeConfiguration<LocationSearch>
    {
        public void Configure(EntityTypeBuilder<LocationSearch> builder)
        {
            builder.ToTable("LocationSearches");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.DisplayName).HasMaxLength(600).IsRequired();
            builder.Property(x => x.SearchText).HasMaxLength(600).IsRequired();
            builder.HasIndex(x => x.AdministrativeAreaId).IsUnique();
            builder.HasIndex(x => x.SearchText);
            builder.HasOne(x => x.AdministrativeArea).WithMany().HasForeignKey(x => x.AdministrativeAreaId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
