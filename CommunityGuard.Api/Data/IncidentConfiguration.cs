using CommunityGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityGuard.Api.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Title)
               .IsRequired()
               .HasMaxLength(200);          // Prevents oversized titles.

        builder.Property(i => i.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(i => i.Sector)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(i => i.Priority)
               .HasConversion<int>();       // Store enum as int, not string.

        builder.Property(i => i.Status)
               .HasConversion<int>();

        // History is owned by the Incident aggregate.
        builder.HasMany(i => i.History)
               .WithOne()
               .HasForeignKey(h => h.IncidentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Common query: open incidents by sector.
        builder.HasIndex(i => new { i.Status, i.Sector });
    }
}