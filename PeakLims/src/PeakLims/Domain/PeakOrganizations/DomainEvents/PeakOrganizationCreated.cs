namespace PeakLims.Domain.PeakOrganizations.DomainEvents;

public sealed class PeakOrganizationCreated : DomainEvent
{
    public PeakOrganization PeakOrganization { get; set; } 
}
            