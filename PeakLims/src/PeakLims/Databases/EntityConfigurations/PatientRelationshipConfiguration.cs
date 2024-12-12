namespace PeakLims.Databases.EntityConfigurations;

using Domain.Ethnicities;
using Domain.PatientRelationships;
using Domain.Races;
using Domain.RelationshipTypes;
using Domain.Sexes;
using PeakLims.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resources;

public sealed class PatientRelationshipConfiguration : IEntityTypeConfiguration<PatientRelationship>
{
    /// <summary>
    /// The database configuration for Patient Relationships
    /// </summary>
    public void Configure(EntityTypeBuilder<PatientRelationship> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasOne(pr => pr.FromPatient)
            .WithMany(p => p.FromRelationships)
            .HasForeignKey(pr => pr.FromPatientId);

        builder.HasOne(pr => pr.ToPatient)
            .WithMany(p => p.ToRelationships)
            .HasForeignKey(pr => pr.ToPatientId);

        builder.Property(x => x.FromRelationship)
            .HasConversion(x => x.Value, x => new RelationshipType(x));
        builder.Property(x => x.ToRelationship)
            .HasConversion(x => x.Value, x => new RelationshipType(x));
    }
}
