using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class CountryAdministrativeLevelConfiguration : IEntityTypeConfiguration<CountryAdministrativeLevel>
{
    public void Configure(EntityTypeBuilder<CountryAdministrativeLevel> builder)
    {
        builder.ToTable("CountryAdministrativeLevels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.DisplayName).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => new { x.CountryId, x.Level, x.DisplayName }).IsUnique();
        builder.HasOne(x => x.Country).WithMany().HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.Restrict);
    }
}
