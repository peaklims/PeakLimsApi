namespace PeakLims.Domain.PanelOrders.DomainEvents;

public sealed class PanelOrderUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            