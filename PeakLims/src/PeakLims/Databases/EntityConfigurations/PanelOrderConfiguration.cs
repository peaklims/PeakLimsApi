namespace PeakLims.Databases.EntityConfigurations;

using Domain.TestOrderCancellationReasons;
using PeakLims.Domain.PanelOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PanelOrderConfiguration : IEntityTypeConfiguration<PanelOrder>
{
    /// <summary>
    /// The database configuration for PanelOrders. 
    /// </summary>
    public void Configure(EntityTypeBuilder<PanelOrder> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasMany(x => x.TestOrders)
            .WithOne(x => x.PanelOrder);
        builder.HasOne(x => x.Panel)
            .WithMany(x => x.PanelOrders);

        builder.Property(x => x.CancellationReason)
            .HasConversion(x => x.Value, x => new CancellationReason(x));
    }
}
