namespace PeakLims.Domain.Accessions;

using SharedKernel.Exceptions;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.Accessions.Models;
using PeakLims.Domain.Accessions.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using AccessionStatuses;
using PeakLims.Domain.Patients;
using PeakLims.Domain.Patients.Models;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Models;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Models;


public class Accession : BaseEntity
{
    public string AccessionNumber { get; private set; }

    public AccessionStatus Status { get; private set; }

    public Patient Patient { get; private set; }

    public HealthcareOrganization HealthcareOrganization { get; private set; }

    private readonly List<HealthcareOrganizationContact> _healthcareOrganizationContact = new();
    public IReadOnlyCollection<HealthcareOrganizationContact> HealthcareOrganizationContacts => _healthcareOrganizationContact.AsReadOnly();

    private readonly List<TestOrder> _testOrder = new();
    public IReadOnlyCollection<TestOrder> TestOrders => _testOrder.AsReadOnly();

    public IReadOnlyCollection<AccessionComment> AccessionComments { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static Accession Create(AccessionForCreation accessionForCreation)
    {
        var newAccession = new Accession();

        newAccession.AccessionNumber = accessionForCreation.AccessionNumber;
        newAccession.Status = AccessionStatus.Draft();

        newAccession.QueueDomainEvent(new AccessionCreated(){ Accession = newAccession });
        
        return newAccession;
    }

    public Accession Update(AccessionForUpdate accessionForUpdate)
    {
        AccessionNumber = accessionForUpdate.AccessionNumber;
        Status = AccessionStatus.Of(accessionForUpdate.Status);

        QueueDomainEvent(new AccessionUpdated(){ Id = Id });
        return this;
    }

    public Accession AddHealthcareOrganizationContact(HealthcareOrganizationContact healthcareOrganizationContact)
    {
        _healthcareOrganizationContact.Add(healthcareOrganizationContact);
        return this;
    }
    
    public Accession RemoveHealthcareOrganizationContact(HealthcareOrganizationContact healthcareOrganizationContact)
    {
        _healthcareOrganizationContact.Remove(healthcareOrganizationContact);
        return this;
    }

    public Accession AddTestOrder(TestOrder testOrder)
    {
        _testOrder.Add(testOrder);
        return this;
    }
    
    public Accession RemoveTestOrder(TestOrder testOrder)
    {
        _testOrder.Remove(testOrder);
        return this;
    }

    public Accession SetPatient(Patient patient)
    {
        Patient = patient;
        return this;
    }

    public Accession SetHealthcareOrganization(HealthcareOrganization healthcareOrganization)
    {
        HealthcareOrganization = healthcareOrganization;
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected Accession() { } // For EF + Mocking
}
