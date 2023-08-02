namespace PeakLims.Databases.EntityConfigurations;

using Domain.TestOrderCancellationReasons;
using Domain.TestOrderStatuses;
using PeakLims.Domain.TestOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TestOrderConfiguration : IEntityTypeConfiguration<TestOrder>
{
    /// <summary>
    /// The database configuration for TestOrders. 
    /// </summary>
    public void Configure(EntityTypeBuilder<TestOrder> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasOne(x => x.Sample)
            .WithMany(x => x.TestOrders);

        builder.Property(x => x.Status)
            .HasConversion(x => x.Value, x => new TestOrderStatus(x));
        builder.Property(x => x.CancellationReason)
            .HasConversion(x => x.Value, x => new TestOrderCancellationReason(x));
        
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
