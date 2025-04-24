namespace PeakLims.Domain.Patients;

using PeakLims.Domain.Accessions;
using PeakLims.Domain.Patients.Models;
using PeakLims.Domain.Patients.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Ethnicities;
using Lifespans;
using PatientRelationships;
using PatientRelationships.Services;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;
using PeakOrganizations;
using Races;
using RelationshipTypes;
using Sexes;
using ValidationException = Exceptions.ValidationException;

public class Patient : BaseEntity
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }
    
    public Lifespan Lifespan { get; private set; }

    public Sex Sex { get; private set; }

    public Race Race { get; private set; }

    public Ethnicity Ethnicity { get; private set; }

    public string InternalId { get; }
    
    public PeakOrganization Organization { get; }
    public Guid OrganizationId { get; private set;  }

    private readonly List<Sample> _sample = new();
    // public IReadOnlyCollection<Sample> Samples => _sample.AsReadOnly();
    public List<Sample> Samples => _sample;

    public List<Accession> Accessions { get; }
    
    public virtual List<PatientRelationship> FromRelationships { get; private set; } 
        = new List<PatientRelationship>();

    public virtual List<PatientRelationship> ToRelationships { get; } 
        = new List<PatientRelationship>();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Patient Create(PatientForCreation patientForCreation)
    {
        var newPatient = new Patient();

        newPatient.FirstName = patientForCreation.FirstName;
        newPatient.LastName = patientForCreation.LastName;
        newPatient.OrganizationId = patientForCreation.OrganizationId;
        newPatient.Lifespan = new Lifespan(patientForCreation.Age, patientForCreation.DateOfBirth);
        newPatient.Sex = Sex.Of(patientForCreation.Sex);
        newPatient.Race = Race.Of(patientForCreation.Race);
        newPatient.Ethnicity = Ethnicity.Of(patientForCreation.Ethnicity);
        
        ValidatePatient(newPatient);

        newPatient.QueueDomainEvent(new PatientCreated(){ Patient = newPatient });
        
        return newPatient;
    }

    private static void ValidatePatient(Patient patient)
    {
        ValidationException.ThrowWhenNullOrWhitespace(patient.FirstName, "Please provide a first name.");
        ValidationException.ThrowWhenNullOrWhitespace(patient.LastName, "Please provide a last name.");
        ValidationException.ThrowWhenNullOrWhitespace(patient.Sex, "Please provide a sex.");
        ValidationException.MustNot(patient.Lifespan.Age == null && patient.Lifespan.DateOfBirth == null, 
            "Please provide a valid age and birth date.");
        ValidationException.ThrowWhenEmpty(patient.OrganizationId, "Please provide an organization id.");
    }

    public Patient Update(PatientForUpdate patientForUpdate)
    {
        FirstName = patientForUpdate.FirstName;
        LastName = patientForUpdate.LastName;
        Lifespan = new Lifespan(patientForUpdate.Age, patientForUpdate.DateOfBirth);
        Sex = Sex.Of(patientForUpdate.Sex);
        Race = Race.Of(patientForUpdate.Race);
        Ethnicity = Ethnicity.Of(patientForUpdate.Ethnicity);
        
        ValidatePatient(this);

        QueueDomainEvent(new PatientUpdated(){ Id = Id });
        return this;
    }

    public Patient AddSample(Sample sample)
    {
        _sample.Add(sample);
        return this;
    }
    
    public Patient RemoveSample(Sample sample)
    {
        _sample.Remove(sample);
        return this;
    }

    public Patient OverrideOrganizationId(Guid organizationId)
    {
        OrganizationId = organizationId;

        QueueDomainEvent(new PatientUpdated(){ Id = Id });
        return this;
    }
    
    public IEnumerable<PatientRelationship> AddRelative(string fromRelationshipType, 
        Patient toRelative, 
        string toRelativeRelationshipType,
        string? notes,
        bool confirmedBidirectional)
    {
        var relationship = PatientRelationshipBuilder
            .For(this)
            .As(RelationshipType.Of(fromRelationshipType))
            .To(toRelative)
            .WhoIs(RelationshipType.Of(toRelativeRelationshipType))
            .WithNotes(notes)
            .Build();
        FromRelationships.Add(relationship);
        
        if(confirmedBidirectional)
        {
            var reverseRelationship = relationship.BuildReverseRelationship();
            toRelative.ToRelationships.Add(reverseRelationship);
            
            return [relationship, reverseRelationship];
        }
        
        return [relationship];
    }

    public void RemoveRelative(PatientRelationship relationship)
    {
        if(FromRelationships.Contains(relationship))
            FromRelationships.RemoveAll(x => x.Id == relationship.Id);
        
        if(ToRelationships.Contains(relationship))
            ToRelationships.RemoveAll(x => x.Id == relationship.Id);

        var bidirectionalFrom = FromRelationships.FirstOrDefault(x => x.FromPatientId == relationship.ToPatientId
                                              && x.ToPatientId == relationship.FromPatientId
                                              && x.FromRelationship == relationship.ToRelationship
                                              && x.ToRelationship == relationship.FromRelationship);
        if(bidirectionalFrom != null)
            FromRelationships.RemoveAll(x => x.Id == bidirectionalFrom.Id);

        var bidirectionalTo = ToRelationships.FirstOrDefault(x => x.FromPatientId == relationship.ToPatientId
                                                                  && x.ToPatientId == relationship.FromPatientId
                                                                  && x.FromRelationship == relationship.ToRelationship
                                                                  && x.ToRelationship == relationship.FromRelationship);
        if(bidirectionalTo != null)
            ToRelationships.RemoveAll(x => x.Id == bidirectionalTo.Id);
    }
    
    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Patient() { } // For EF + Mocking
}
