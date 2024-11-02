namespace PeakLims.Databases.EntityConfigurations;

using Domain;
using Domain.PanelStatuses;
using PeakLims.Domain.Panels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PanelTestAssignmentConfiguration : IEntityTypeConfiguration<PanelTestAssignment>
{
    /// <summary>
    /// The database configuration for Panels. 
    /// </summary>
    public void Configure(EntityTypeBuilder<PanelTestAssignment> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder
            .HasKey(pt => new { pt.PanelId, pt.TestId });
        
        builder
            .HasOne(pt => pt.Panel)
            .WithMany(p => p.TestAssignments)
            .HasForeignKey(pt => pt.PanelId);

        builder
            .HasOne(pt => pt.Test)
            .WithMany(t => t.PanelTestAssignments)
            .HasForeignKey(pt => pt.TestId);
    }
}
