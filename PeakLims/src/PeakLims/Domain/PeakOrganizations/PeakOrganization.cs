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

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static PeakOrganization Create(PeakOrganizationForCreation peakOrganizationForCreation)
    {
        var newPeakOrganization = new PeakOrganization();

        newPeakOrganization.Name = peakOrganizationForCreation.Name;

        newPeakOrganization.QueueDomainEvent(new PeakOrganizationCreated(){ PeakOrganization = newPeakOrganization });
        
        return newPeakOrganization;
    }

    public PeakOrganization Update(PeakOrganizationForUpdate peakOrganizationForUpdate)
    {
        Name = peakOrganizationForUpdate.Name;

        QueueDomainEvent(new PeakOrganizationUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected PeakOrganization() { } // For EF + Mocking
}