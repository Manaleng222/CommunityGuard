using CommunityGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityGuard.Api.Data.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(r => r.Type)
               .HasConversion<int>();

        // Resource names must be unique — no two "Patrol 1".
        builder.HasIndex(r => r.Name)
               .IsUnique();

        // Allocations are loaded through the Resource aggregate.
        builder.HasMany(r => r.Allocations)
               .WithOne()
               .HasForeignKey(a => a.ResourceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}