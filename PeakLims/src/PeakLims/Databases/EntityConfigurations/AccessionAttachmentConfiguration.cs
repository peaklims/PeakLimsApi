namespace PeakLims.Databases.EntityConfigurations;

using PeakLims.Domain.AccessionAttachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class AccessionAttachmentConfiguration : IEntityTypeConfiguration<AccessionAttachment>
{
    /// <summary>
    /// The database configuration for AccessionAttachments. 
    /// </summary>
    public void Configure(EntityTypeBuilder<AccessionAttachment> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        
        builder.OwnsOne(x => x.Type, opts =>
            {
                opts.Property(x => x.Value).HasColumnName("type");
            }).Navigation(x => x.Type)
            .IsRequired();
        
        builder.OwnsOne(x => x.S3Key, opts =>
            {
                opts.Property(x => x.Value).HasColumnName("s3key");
            }).Navigation(x => x.S3Key)
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