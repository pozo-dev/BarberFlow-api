using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberFlow.Infrastructure.Persistence.Configurations
{
    public class CollaboratorConfiguration : IEntityTypeConfiguration<Collaborator>
    {
        public void Configure(EntityTypeBuilder<Collaborator> builder)
        {
            builder.ToTable("Collaborators");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).HasMaxLength(120).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(30).IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            builder.HasOne(x => x.Branch)
                .WithMany()
                .HasForeignKey(x => x.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserProfile)
                .WithMany(x => x.Collaborators)
                .HasForeignKey(x => x.UserProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.BranchId, x.IsActive });
            builder.HasIndex(x => x.UserProfileId);
        }
    }
}
