using Brainz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brainz.Infrastructure.Data.Configurations;

public class SyncLogConfiguration : IEntityTypeConfiguration<SyncLog>
{
    public void Configure(EntityTypeBuilder<SyncLog> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ErrorMessage).HasMaxLength(2000);
        builder.Property(s => s.Details).HasMaxLength(4000);
        builder.HasIndex(s => s.StartedAt);
    }
}
