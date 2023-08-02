namespace PeakLims.Databases.EntityConfigurations;

using Domain.SampleTypes;
using PeakLims.Domain.Samples;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resources;

public sealed class SampleConfiguration : IEntityTypeConfiguration<Sample>
{
    /// <summary>
    /// The database configuration for Samples. 
    /// </summary>
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasOne(x => x.ParentSample);
        builder.HasOne(x => x.Container)
            .WithMany(x => x.Samples);
        
        builder.Property(o => o.SampleNumber)
            .HasDefaultValueSql($"concat('{Consts.DatabaseSequences.SampleNumberPrefix}', nextval('\"{Consts.DatabaseSequences.SampleNumberPrefix}\"'))")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion(x => x.Value, x => new SampleType(x));
        
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
