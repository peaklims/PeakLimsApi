namespace PeakLims.Databases.EntityConfigurations;

using Domain.Ethnicities;
using Domain.Races;
using Domain.Sexes;
using PeakLims.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resources;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    /// <summary>
    /// The database configuration for Patients. 
    /// </summary>
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        // Relationship Marker -- Deleting or modifying this comment could cause incomplete relationship scaffolding
        builder.HasMany(x => x.Samples)
            .WithOne(x => x.Patient);
        
        builder.Property(o => o.InternalId)
            .HasDefaultValueSql($"concat('{Consts.DatabaseSequences.PatientInternalIdPrefix}', nextval('\"{Consts.DatabaseSequences.PatientInternalIdPrefix}\"'))")
            .IsRequired();

        builder.Property(x => x.Sex)
            .HasConversion(x => x.Value, x => new Sex(x));
        builder.Property(x => x.Race)
            .HasConversion(x => x.Value, x => new Race(x));
        builder.Property(x => x.Ethnicity)
            .HasConversion(x => x.Value, x => new Ethnicity(x));
        builder.OwnsOne(x => x.Lifespan, opts =>
            {
                opts.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
                opts.Property(x => x.KnownAge).HasColumnName("known_age");
            }).Navigation(x => x.Lifespan)
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
