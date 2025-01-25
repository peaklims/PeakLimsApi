namespace PeakLims.Domain.WorldBuildingAttempts;

using System.ComponentModel.DataAnnotations;
using PeakLims.Domain.WorldBuildingPhases;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Destructurama.Attributed;
using PeakLims.Exceptions;
using PeakLims.Domain.WorldBuildingAttempts.DomainEvents;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Domain.WorldBuildingStatuses;
using WorldBuildingPhaseNames;
using ValidationException = Exceptions.ValidationException;

public class WorldBuildingAttempt : BaseEntity
{
   public WorldBuildingStatus Status { get; private set; }

    private readonly List<WorldBuildingPhase> _worldBuildingPhases = new();
    public IReadOnlyCollection<WorldBuildingPhase> WorldBuildingPhases => _worldBuildingPhases.AsReadOnly();

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static WorldBuildingAttempt CreateStandardWorld()
    {
        var newWorldBuildingAttempt = new WorldBuildingAttempt();

        newWorldBuildingAttempt.Status = WorldBuildingStatus.Pending();
        newWorldBuildingAttempt
            .AddWorldBuildingPhase(WorldBuildingPhaseName.CreateOrganization()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GenerateUsers()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GenerateHealthcareOrganizations()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GenerateHealthcareOrganizationContacts()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GeneratePanelsAndTests()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.AddDefaultContainers()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GeneratePatients()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.GenerateAccessions()
                .CreateInitialPhaseState())
            .AddWorldBuildingPhase(WorldBuildingPhaseName.FinalizeInDatabase()
                .CreateInitialPhaseState());
        
        newWorldBuildingAttempt.QueueDomainEvent(new WorldBuildingAttemptCreated(){ WorldBuildingAttempt = newWorldBuildingAttempt });
        
        return newWorldBuildingAttempt;
    }

    public WorldBuildingAttempt AddWorldBuildingPhase(WorldBuildingPhase worldBuildingPhase)
    {
        worldBuildingPhase.SetStepNumber(_worldBuildingPhases.Count + 1);
        _worldBuildingPhases.Add(worldBuildingPhase);
        return this;
    }
    
    public WorldBuildingAttempt RemoveWorldBuildingPhase(WorldBuildingPhase worldBuildingPhase)
    {
        _worldBuildingPhases.RemoveAll(x => x.Id == worldBuildingPhase.Id);
        return this;
    }
    
    public WorldBuildingAttempt StartPhase(WorldBuildingPhaseName worldBuildingPhaseName, string specialRequest)
    {
        var worldBuildingPhase = _worldBuildingPhases
            .FirstOrDefault(x => x.Name == worldBuildingPhaseName);
        if (worldBuildingPhase == null)
            throw new ValidationException($"World Building Phase {worldBuildingPhaseName} not found in this attempt.");
        
        if (!string.IsNullOrWhiteSpace(specialRequest))
            worldBuildingPhase.SetSpecialRequests(specialRequest);
        
        worldBuildingPhase.Start();
        RecomputeStatus();
        return this;
    }
    
    public WorldBuildingAttempt SuccessfullyEndPhase(WorldBuildingPhaseName worldBuildingPhaseName, object resultData)
    {
        var worldBuildingPhase = _worldBuildingPhases
            .FirstOrDefault(x => x.Name == worldBuildingPhaseName);
        if (worldBuildingPhase == null)
            throw new ValidationException($"World Building Phase {worldBuildingPhaseName} not found in this attempt.");
        
        if(resultData != null)
            worldBuildingPhase.SetResultData(resultData);
        
        worldBuildingPhase.MarkSuccessful();
        RecomputeStatus();
        return this;
    }

    public WorldBuildingAttempt FailPhase(WorldBuildingPhaseName worldBuildingPhaseName, string errorMessage)
    {
        var worldBuildingPhase = _worldBuildingPhases
            .FirstOrDefault(x => x.Name == worldBuildingPhaseName);
        if (worldBuildingPhase == null)
            throw new ValidationException($"World Building Phase {worldBuildingPhaseName} not found in this attempt.");
        
        worldBuildingPhase.Fail(errorMessage);
        RecomputeStatus();
        return this;
    }


    public void RecomputeStatus()
    {
        Status = _worldBuildingPhases.Count switch
        {
            0 => WorldBuildingStatus.Pending(),
            _ when _worldBuildingPhases.All(x => x.Status == WorldBuildingStatus.Successful())
                => WorldBuildingStatus.Successful(),
            _ when _worldBuildingPhases.Any(x => x.Status == WorldBuildingStatus.Failed())
                => WorldBuildingStatus.Failed(),
            _ => WorldBuildingStatus.Processing()
        };
    }
    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    protected WorldBuildingAttempt() { } // For EF + Mocking
}
