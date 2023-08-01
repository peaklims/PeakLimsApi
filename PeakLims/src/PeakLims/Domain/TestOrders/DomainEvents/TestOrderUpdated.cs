namespace PeakLims.Domain.TestOrders.DomainEvents;

public sealed class TestOrderUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            