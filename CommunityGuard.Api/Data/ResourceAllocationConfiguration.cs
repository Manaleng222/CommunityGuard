using CommunityGuard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommunityGuard.Api.Data.Configurations;

public class ResourceAllocationConfiguration : IEntityTypeConfiguration<ResourceAllocation>
{
    public void Configure(EntityTypeBuilder<ResourceAllocation> builder)
    {
        builder.ToTable("ResourceAllocations");
        builder.HasKey(a => a.Id);

        // Index for overlap lookups: "is this resource booked between X and Y?"
        builder.HasIndex(a => new { a.ResourceId, a.FromUtc, a.ToUtc });
    }
}