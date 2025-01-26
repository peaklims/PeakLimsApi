namespace PeakLims.Domain.PeakOrganizations;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.PeakOrganizations.Models;
using PeakLims.Domain.PeakOrganizations.DomainEvents;


public class PeakOrganization : BaseEntity
{
    public string Name { get; private set; }
    public string Domain { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static PeakOrganization Create(PeakOrganizationForCreation peakOrganizationForCreation)
    {
        var newPeakOrganization = new PeakOrganization();

        newPeakOrganization.Name = peakOrganizationForCreation.Name;
        newPeakOrganization.Domain = peakOrganizationForCreation.Domain;

        newPeakOrganization.QueueDomainEvent(new PeakOrganizationCreated(){ PeakOrganization = newPeakOrganization });
        
        return newPeakOrganization;
    }
    
    public static PeakOrganization Create(Guid organizationId, PeakOrganizationForCreation peakOrganizationForCreation)
    {
        var newPeakOrganization = new PeakOrganization();

        newPeakOrganization.OverrideId(organizationId);
        newPeakOrganization.Name = peakOrganizationForCreation.Name;
        newPeakOrganization.Domain = peakOrganizationForCreation.Domain;

        newPeakOrganization.QueueDomainEvent(new PeakOrganizationCreated(){ PeakOrganization = newPeakOrganization });
        
        return newPeakOrganization;
    }

    public PeakOrganization Update(PeakOrganizationForUpdate peakOrganizationForUpdate)
    {
        Name = peakOrganizationForUpdate.Name;
        Domain = peakOrganizationForUpdate.Domain;

        QueueDomainEvent(new PeakOrganizationUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected PeakOrganization() { } // For EF + Mocking
}