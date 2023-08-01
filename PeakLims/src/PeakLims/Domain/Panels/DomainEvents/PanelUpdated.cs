namespace PeakLims.Domain.Panels.DomainEvents;

public sealed class PanelUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            