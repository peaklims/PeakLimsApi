namespace PeakLims.Domain.HealthcareOrganizationContacts;

using SharedKernel.Exceptions;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;
using PeakLims.Domain.HealthcareOrganizationContacts.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Models;


public class HealthcareOrganizationContact : BaseEntity
{
    public string Name { get; private set; }

    public string Email { get; private set; }

    public string Npi { get; private set; }

    private readonly List<HealthcareOrganization> _healthcareOrganization = new();
    public IReadOnlyCollection<HealthcareOrganization> HealthcareOrganizations => _healthcareOrganization.AsReadOnly();

    public Accession Accession { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static HealthcareOrganizationContact Create(HealthcareOrganizationContactForCreation healthcareOrganizationContactForCreation)
    {
        var newHealthcareOrganizationContact = new HealthcareOrganizationContact();

        newHealthcareOrganizationContact.Name = healthcareOrganizationContactForCreation.Name;
        newHealthcareOrganizationContact.Email = healthcareOrganizationContactForCreation.Email;
        newHealthcareOrganizationContact.Npi = healthcareOrganizationContactForCreation.Npi;

        newHealthcareOrganizationContact.QueueDomainEvent(new HealthcareOrganizationContactCreated(){ HealthcareOrganizationContact = newHealthcareOrganizationContact });
        
        return newHealthcareOrganizationContact;
    }

    public HealthcareOrganizationContact Update(HealthcareOrganizationContactForUpdate healthcareOrganizationContactForUpdate)
    {
        Name = healthcareOrganizationContactForUpdate.Name;
        Email = healthcareOrganizationContactForUpdate.Email;
        Npi = healthcareOrganizationContactForUpdate.Npi;

        QueueDomainEvent(new HealthcareOrganizationContactUpdated(){ Id = Id });
        return this;
    }

    public HealthcareOrganizationContact AddHealthcareOrganization(HealthcareOrganization healthcareOrganization)
    {
        _healthcareOrganization.Add(healthcareOrganization);
        return this;
    }
    
    public HealthcareOrganizationContact RemoveHealthcareOrganization(HealthcareOrganization healthcareOrganization)
    {
        _healthcareOrganization.Remove(healthcareOrganization);
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected HealthcareOrganizationContact() { } // For EF + Mocking
}
