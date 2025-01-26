namespace PeakLims.Domain.WorldBuildingAttempts.DomainEvents;

public sealed class WorldBuildingAttemptCreated : DomainEvent
{
    public WorldBuildingAttempt WorldBuildingAttempt { get; set; } 
}
            