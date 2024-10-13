namespace PeakLims.Databases.EntityConfigurations;

using PeakLims.Domain.HealthcareOrganizationContacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class HealthcareOrganizationContactConfiguration : IEntityTypeConfiguration<HealthcareOrganizationContact>
{
    /// <summary>
    /// The database configuration for HealthcareOrganizationContacts. 
    /// </summary>
    public void Configure(EntityTypeBuilder<HealthcareOrganizationContact> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasOne(x => x.HealthcareOrganization)
            .WithMany(x => x.HealthcareOrganizationContacts);
        
        builder.OwnsOne(x => x.Npi, opts =>
            {
                opts.Property(x => x.Value).HasColumnName("npi");
            }).Navigation(x => x.Npi)
            .IsRequired();
    }
}
