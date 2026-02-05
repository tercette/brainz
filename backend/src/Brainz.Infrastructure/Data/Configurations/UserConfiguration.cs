using Brainz.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brainz.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.MicrosoftId).IsRequired().HasMaxLength(128);
        builder.HasIndex(u => u.MicrosoftId).IsUnique();
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(320);
        builder.HasIndex(u => u.Email);
        builder.Property(u => u.GivenName).HasMaxLength(128);
        builder.Property(u => u.Surname).HasMaxLength(128);
        builder.Property(u => u.Department).HasMaxLength(128);
        builder.Property(u => u.JobTitle).HasMaxLength(128);

        builder.HasMany(u => u.Events)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
