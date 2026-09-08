using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class AdministrativeAreaTypeConfiguration : IEntityTypeConfiguration<AdministrativeAreaType>
{
    public void Configure(EntityTypeBuilder<AdministrativeAreaType> builder)
    {
        builder.ToTable("AdministrativeAreaTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => new { x.CountryAdministrativeLevelId, x.Code }).IsUnique();
        builder.HasOne(x => x.CountryAdministrativeLevel).WithMany().HasForeignKey(x => x.CountryAdministrativeLevelId).OnDelete(DeleteBehavior.Restrict);
    }
}
