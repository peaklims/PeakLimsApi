namespace PeakLims.Domain.AccessionComments.DomainEvents;

public sealed class AccessionCommentUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            