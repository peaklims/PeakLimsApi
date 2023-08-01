namespace PeakLims.Domain.AccessionComments.DomainEvents;

public sealed class AccessionCommentCreated : DomainEvent
{
    public AccessionComment AccessionComment { get; set; } 
}
            