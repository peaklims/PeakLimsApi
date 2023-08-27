namespace PeakLims.Domain.AccessionContacts.DomainEvents;

public sealed class AccessionContactCreated : DomainEvent
{
    public AccessionContact AccessionContact { get; set; } 
}
            