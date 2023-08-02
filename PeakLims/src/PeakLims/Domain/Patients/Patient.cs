namespace PeakLims.Domain.Patients;

using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Patients.Models;
using PeakLims.Domain.Patients.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Lifespans;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;
using Sexes;

public class Patient : BaseEntity
{
    public string FirstName { get; private set; }

    public string LastName { get; private set; }
    
    public virtual Lifespan Lifespan { get; private set; }

    public Sex Sex { get; private set; }

    public string Race { get; private set; }

    public string Ethnicity { get; private set; }

    public string InternalId { get; private set; }

    private readonly List<Sample> _sample = new();
    public IReadOnlyCollection<Sample> Samples => _sample.AsReadOnly();

    public IReadOnlyCollection<Accession> Accessions { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Patient Create(PatientForCreation patientForCreation)
    {
        var newPatient = new Patient();

        newPatient.FirstName = patientForCreation.FirstName;
        newPatient.LastName = patientForCreation.LastName;
        newPatient.Lifespan = new Lifespan(patientForCreation.Age, patientForCreation.DateOfBirth);
        newPatient.Sex = Sex.Of(patientForCreation.Sex);
        newPatient.Race = patientForCreation.Race;
        newPatient.Ethnicity = patientForCreation.Ethnicity;
        newPatient.InternalId = patientForCreation.InternalId;

        newPatient.QueueDomainEvent(new PatientCreated(){ Patient = newPatient });
        
        return newPatient;
    }

    public Patient Update(PatientForUpdate patientForUpdate)
    {
        FirstName = patientForUpdate.FirstName;
        LastName = patientForUpdate.LastName;
        Lifespan = new Lifespan(patientForUpdate.Age, patientForUpdate.DateOfBirth);
        Sex = Sex.Of(patientForUpdate.Sex);
        Race = patientForUpdate.Race;
        Ethnicity = patientForUpdate.Ethnicity;
        InternalId = patientForUpdate.InternalId;

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

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Patient() { } // For EF + Mocking
}
