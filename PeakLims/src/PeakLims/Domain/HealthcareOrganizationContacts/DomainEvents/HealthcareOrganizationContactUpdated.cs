namespace PeakLims.Domain.HealthcareOrganizationContacts.DomainEvents;

public sealed class HealthcareOrganizationContactUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            