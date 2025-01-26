namespace PeakLims.Domain.WorldBuildingPhases.DomainEvents;

public sealed class WorldBuildingPhaseCreated : DomainEvent
{
    public WorldBuildingPhase WorldBuildingPhase { get; set; } 
}
            