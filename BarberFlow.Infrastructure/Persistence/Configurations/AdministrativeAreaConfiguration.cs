using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations;
public class AdministrativeAreaConfiguration : IEntityTypeConfiguration<AdministrativeArea>
{
    public void Configure(EntityTypeBuilder<AdministrativeArea> builder)
    {
        builder.ToTable("AdministrativeAreas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();
        builder.Property(x => x.Code).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.CountryAdministrativeLevelId, x.ParentId, x.Code }).IsUnique();
        builder.HasOne(x => x.CountryAdministrativeLevel).WithMany().HasForeignKey(x => x.CountryAdministrativeLevelId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AdministrativeAreaType).WithMany().HasForeignKey(x => x.AdministrativeAreaTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);
    }
}
