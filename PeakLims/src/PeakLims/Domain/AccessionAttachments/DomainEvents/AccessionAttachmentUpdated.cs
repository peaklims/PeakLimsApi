namespace PeakLims.Domain.AccessionAttachments.DomainEvents;

public sealed class AccessionAttachmentUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            