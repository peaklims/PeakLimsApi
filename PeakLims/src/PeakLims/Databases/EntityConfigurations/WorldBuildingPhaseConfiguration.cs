namespace PeakLims.Databases.EntityConfigurations;

using Domain.WorldBuildingPhaseNames;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Domain.WorldBuildingStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class WorldBuildingPhaseConfiguration : IEntityTypeConfiguration<WorldBuildingPhase>
{
    /// <summary>
    /// The database configuration for WorldBuildingPhases. 
    /// </summary>
    public void Configure(EntityTypeBuilder<WorldBuildingPhase> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding

        // Property Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding

        builder.Property(x => x.Status)
            .HasConversion(x => x.Value, x => new WorldBuildingStatus(x));

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new WorldBuildingPhaseName(x));
        
        // example for a more complex value object
        // builder.OwnsOne(x => x.PhysicalAddress, opts =>
        // {
        //     opts.Property(x => x.Line1).HasColumnName("physical_address_line1");
        //     opts.Property(x => x.Line2).HasColumnName("physical_address_line2");
        //     opts.Property(x => x.City).HasColumnName("physical_address_city");
        //     opts.Property(x => x.State).HasColumnName("physical_address_state");
        //     opts.OwnsOne(x => x.PostalCode, o =>
        //         {
        //             o.Property(x => x.Value).HasColumnName("physical_address_postal_code");
        //         }).Navigation(x => x.PostalCode)
        //         .IsRequired();
        //     opts.Property(x => x.Country).HasColumnName("physical_address_country");
        // }).Navigation(x => x.PhysicalAddress)
        //     .IsRequired();
    }
}
