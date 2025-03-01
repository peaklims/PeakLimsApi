namespace PeakLims.Databases.EntityConfigurations;

using Domain.AccessionStatuses;
using PeakLims.Domain.Accessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resources;

public sealed class AccessionConfiguration : IEntityTypeConfiguration<Accession>
{
    /// <summary>
    /// The database configuration for Accessions. 
    /// </summary>
    public void Configure(EntityTypeBuilder<Accession> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasMany(x => x.AccessionAttachments)
            .WithOne(x => x.Accession);
        builder.HasMany(x => x.AccessionContacts)
            .WithOne(x => x.Accession);
        builder.HasOne(x => x.Patient)
            .WithMany(x => x.Accessions);
        builder.HasOne(x => x.HealthcareOrganization)
            .WithMany(x => x.Accessions);
        builder.HasMany(x => x.TestOrders)
            .WithOne(x => x.Accession);
        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId);
        
        builder.Property(o => o.AccessionNumber)
            .HasDefaultValueSql($"concat('{Consts.DatabaseSequences.AccessionNumberPrefix}', nextval('\"{Consts.DatabaseSequences.AccessionNumberPrefix}\"'))")
            .IsRequired();

        builder.OwnsOne(x => x.Status, opts =>
            {
                opts.Property(x => x.Value).HasColumnName("status");
            }).Navigation(x => x.Status)
            .IsRequired();
        
        // example for a more complex value object
        // builder.OwnsOne(x => x.PhysicalAddress, opts =>
        // {
        //     opts.Property(x => x.Line1).HasColumnName("physical_address_line1");
        //     opts.Property(x => x.Line2).HasColumnName("physical_address_line2");
        //     opts.Property(x => x.City).HasColumnName("physical_address_city");
        //     opts.Property(x => x.State).HasColumnName("physical_address_state");
        //     opts.Property(x => x.PostalCode).HasColumnName("physical_address_postal_code")
        //         .HasConversion(x => x.Value, x => new PostalCode(x));
        //     opts.Property(x => x.Country).HasColumnName("physical_address_country");
        // }).Navigation(x => x.PhysicalAddress)
        //     .IsRequired();
    }
}
