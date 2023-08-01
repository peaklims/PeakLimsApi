namespace PeakLims.Domain.Panels.DomainEvents;

public sealed class PanelCreated : DomainEvent
{
    public Panel Panel { get; set; } 
}
            