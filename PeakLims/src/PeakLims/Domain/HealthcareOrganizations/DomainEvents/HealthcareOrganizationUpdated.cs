namespace PeakLims.Domain.HealthcareOrganizations.DomainEvents;

public sealed class HealthcareOrganizationUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            