namespace PeakLims.Domain.AccessionContacts.DomainEvents;

public sealed class AccessionContactUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            