using Brainz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brainz.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.MicrosoftEventId).IsRequired().HasMaxLength(512);
        builder.HasIndex(e => e.MicrosoftEventId);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(512);
        builder.Property(e => e.BodyPreview).HasMaxLength(1000);
        builder.Property(e => e.Location).HasMaxLength(512);
        builder.Property(e => e.OrganizerName).HasMaxLength(256);
        builder.Property(e => e.OrganizerEmail).HasMaxLength(320);
        builder.HasIndex(e => new { e.UserId, e.StartDateTime });
    }
}
