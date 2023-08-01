namespace PeakLims.Domain.Accessions.DomainEvents;

public sealed class AccessionUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            