namespace PeakLims.Domain.HealthcareOrganizations;

using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.HealthcareOrganizations.Models;
using PeakLims.Domain.HealthcareOrganizations.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using HealthcareOrganizationStatuses;

public class HealthcareOrganization : BaseEntity
{
    public string Name { get; private set; }

    public string Email { get; private set; }
    
    /// <summary>
    /// The tenant organization id that this healthcare organization belongs to
    /// </summary>
    public Guid OrganizationId { get; private set; }
    
    public HealthcareOrganizationStatus Status { get; private set; }

    public IReadOnlyCollection<Accession> Accessions { get; }


    private readonly List<HealthcareOrganizationContact> _organizationContacts = new();
    // public IReadOnlyCollection<HealthcareOrganizationContact> HealthcareOrganizationContacts => _organizationContacts.AsReadOnly();
    // TODO readonly collection is causing issues with joins in aggreagates when joining org to things
    public List<HealthcareOrganizationContact> HealthcareOrganizationContacts => _organizationContacts;

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static HealthcareOrganization Create(HealthcareOrganizationForCreation healthcareOrganizationForCreation)
    {
        var newHealthcareOrganization = new HealthcareOrganization();

        newHealthcareOrganization.Name = healthcareOrganizationForCreation.Name;
        newHealthcareOrganization.Email = healthcareOrganizationForCreation.Email;
        newHealthcareOrganization.Status = HealthcareOrganizationStatus.Active();
        newHealthcareOrganization.OrganizationId = healthcareOrganizationForCreation.OrganizationId;

        newHealthcareOrganization.QueueDomainEvent(new HealthcareOrganizationCreated(){ HealthcareOrganization = newHealthcareOrganization });
        
        return newHealthcareOrganization;
    }

    public HealthcareOrganization Update(HealthcareOrganizationForUpdate healthcareOrganizationForUpdate)
    {
        Name = healthcareOrganizationForUpdate.Name;
        Email = healthcareOrganizationForUpdate.Email;

        QueueDomainEvent(new HealthcareOrganizationUpdated(){ Id = Id });
        return this;
    }

    public HealthcareOrganization Activate()
    {
        if (Status == HealthcareOrganizationStatus.Active())
            return this;
        
        Status = HealthcareOrganizationStatus.Active();
        QueueDomainEvent(new HealthcareOrganizationUpdated(){ Id = Id });
        return this;
    }

    public HealthcareOrganization Deactivate()
    {
        if (Status == HealthcareOrganizationStatus.Inactive())
            return this;
        
        Status = HealthcareOrganizationStatus.Inactive();
        QueueDomainEvent(new HealthcareOrganizationUpdated(){ Id = Id });
        return this;
    }

    public HealthcareOrganization AddContact(HealthcareOrganizationContact contact)
    {
        var alreadyExists = HealthcareOrganizationContactAlreadyExists(contact);
        if (alreadyExists)
            return this;
        
        _organizationContacts.Add(contact);
        QueueDomainEvent(new HealthcareOrganizationUpdated(){ Id = Id });
        return this;
    }

    public HealthcareOrganization RemoveContact(HealthcareOrganizationContact contact)
    {
        var alreadyExists = HealthcareOrganizationContactAlreadyExists(contact);
        if (!alreadyExists)
            return this;
        
        _organizationContacts.RemoveAll(x => x.Id == contact.Id);
        QueueDomainEvent(new HealthcareOrganizationUpdated(){ Id = Id });
        return this;
    }

    private bool HealthcareOrganizationContactAlreadyExists(HealthcareOrganizationContact contact) 
        => _organizationContacts.Any(x => contact.Id == x.Id);

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected HealthcareOrganization() { } // For EF + Mocking
}
