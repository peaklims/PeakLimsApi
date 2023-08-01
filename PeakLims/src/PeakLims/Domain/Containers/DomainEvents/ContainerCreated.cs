namespace PeakLims.Domain.Containers.DomainEvents;

public sealed class ContainerCreated : DomainEvent
{
    public Container Container { get; set; } 
}
            