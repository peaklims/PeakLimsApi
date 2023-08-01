namespace PeakLims.Domain.HealthcareOrganizationContacts.DomainEvents;

public sealed class HealthcareOrganizationContactCreated : DomainEvent
{
    public HealthcareOrganizationContact HealthcareOrganizationContact { get; set; } 
}
            