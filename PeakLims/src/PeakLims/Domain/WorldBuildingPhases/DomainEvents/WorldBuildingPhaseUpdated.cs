namespace PeakLims.Domain.WorldBuildingPhases.DomainEvents;

public sealed class WorldBuildingPhaseUpdated : DomainEvent
{
    public Guid Id { get; set; } 
}
            