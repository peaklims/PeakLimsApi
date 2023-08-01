namespace PeakLims.Databases.EntityConfigurations;

using Domain.Users;
using Domain.Emails;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// The database configuration for Users. 
    /// </summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding

        builder.Property(x => x.Email)
            .HasConversion(x => x.Value, x => new Email(x))
            .HasColumnName("email");
    }
}