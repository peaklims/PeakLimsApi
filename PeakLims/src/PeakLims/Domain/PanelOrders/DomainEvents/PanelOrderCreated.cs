namespace PeakLims.Domain.PanelOrders.DomainEvents;

public sealed class PanelOrderCreated : DomainEvent
{
    public PanelOrder PanelOrder { get; set; } 
}
            