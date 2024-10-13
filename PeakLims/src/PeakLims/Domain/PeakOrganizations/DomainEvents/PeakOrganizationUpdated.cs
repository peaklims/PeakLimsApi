namespace PeakLims.Domain.PeakOrganizations.DomainEvents;

public sealed class PeakOrganizationUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            