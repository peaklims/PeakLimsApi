namespace PeakLims.Domain.Tests.DomainEvents;

public sealed class TestCreated : DomainEvent
{
    public Test Test { get; set; } 
}
            