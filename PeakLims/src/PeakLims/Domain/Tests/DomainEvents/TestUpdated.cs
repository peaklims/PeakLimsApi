namespace PeakLims.Domain.Tests.DomainEvents;

public sealed class TestUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            