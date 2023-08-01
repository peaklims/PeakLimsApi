namespace PeakLims.Domain.Containers.DomainEvents;

public sealed class ContainerUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            