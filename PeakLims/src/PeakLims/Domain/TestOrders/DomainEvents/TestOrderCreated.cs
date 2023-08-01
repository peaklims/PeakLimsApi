namespace PeakLims.Domain.TestOrders.DomainEvents;

public sealed class TestOrderCreated : DomainEvent
{
    public TestOrder TestOrder { get; set; } 
}
            