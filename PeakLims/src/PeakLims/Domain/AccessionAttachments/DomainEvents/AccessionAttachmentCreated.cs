namespace PeakLims.Domain.AccessionAttachments.DomainEvents;

public sealed class AccessionAttachmentCreated : DomainEvent
{
    public AccessionAttachment AccessionAttachment { get; set; } 
}
            