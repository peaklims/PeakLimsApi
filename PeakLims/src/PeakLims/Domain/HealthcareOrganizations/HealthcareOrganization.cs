namespace PeakLims.Domain.HealthcareOrganizations;

using SharedKernel.Exceptions;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.HealthcareOrganizations.Models;
using PeakLims.Domain.HealthcareOrganizations.DomainEvents;
using FluentValidation;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


public class HealthcareOrganization : BaseEntity
{
    public string Name { get; private set; }

    public string Email { get; private set; }

    public IReadOnlyCollection<Accession> Accessions { get; }

    public IReadOnlyCollection<HealthcareOrganizationContact> HealthcareOrganizationContacts { get; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static HealthcareOrganization Create(HealthcareOrganizationForCreation healthcareOrganizationForCreation)
    {
        var newHealthcareOrganization = new HealthcareOrganization();

        newHealthcareOrganization.Name = healthcareOrganizationForCreation.Name;
        newHealthcareOrganization.Email = healthcareOrganizationForCreation.Email;

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

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected HealthcareOrganization() { } // For EF + Mocking
}
