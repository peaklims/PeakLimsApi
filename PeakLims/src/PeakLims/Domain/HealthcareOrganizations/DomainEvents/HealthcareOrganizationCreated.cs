namespace PeakLims.Domain.HealthcareOrganizations.DomainEvents;

public sealed class HealthcareOrganizationCreated : DomainEvent
{
    public HealthcareOrganization HealthcareOrganization { get; set; } 
}
            