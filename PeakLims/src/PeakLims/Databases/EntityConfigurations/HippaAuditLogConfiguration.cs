namespace PeakLims.Databases.EntityConfigurations;

using PeakLims.Domain.HipaaAuditLogs;
using PeakLims.Domain.AuditLogActions;
using PeakLims.Domain.AuditLogConcepts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class HipaaAuditLogConfiguration : IEntityTypeConfiguration<HipaaAuditLog>
{
    /// <summary>
    /// The database configuration for HipaaAuditLogs. 
    /// </summary>
    public void Configure(EntityTypeBuilder<HipaaAuditLog> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding

        // Property Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding

        builder.Property(x => x.Action)
            .HasConversion(x => x.Value, x => new AuditLogAction(x));

        builder.Property(x => x.Concept)
            .HasConversion(x => x.Value, x => new AuditLogConcept(x));
        
        builder.Property(x => x.Data).HasColumnType("jsonb");
    }
}
