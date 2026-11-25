using CommunityGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityGuard.Api.Data.Configurations;

public class IncidentStatusHistoryConfiguration : IEntityTypeConfiguration<IncidentStatusHistory>
{
    public void Configure(EntityTypeBuilder<IncidentStatusHistory> builder)
    {
        builder.ToTable("IncidentStatusHistories");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Status)
               .HasConversion<int>();

        builder.Property(h => h.Note)
               .HasMaxLength(500);

        // Index for response-time calculations (dashboard aggregates).
        builder.HasIndex(h => new { h.IncidentId, h.ChangedAt });
    }
}