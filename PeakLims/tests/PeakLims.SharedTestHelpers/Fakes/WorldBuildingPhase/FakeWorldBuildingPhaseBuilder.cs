namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;

using PeakLims.Domain.WorldBuildingPhases;

public class FakeWorldBuildingPhaseBuilder
{
    private WorldBuildingPhaseForCreation _creationData = new FakeWorldBuildingPhaseForCreation().Generate();

    public FakeWorldBuildingPhaseBuilder WithModel(WorldBuildingPhaseForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithResultData(string resultData)
    {
        _creationData.ResultData = resultData;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithStartedAt(DateTimeOffset? startedAt)
    {
        _creationData.StartedAt = startedAt;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithEndedAt(DateTimeOffset? endedAt)
    {
        _creationData.EndedAt = endedAt;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithStepNumber(int stepNumber)
    {
        _creationData.StepNumber = stepNumber;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithComments(string comments)
    {
        _creationData.Comments = comments;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithErrorMessage(string errorMessage)
    {
        _creationData.ErrorMessage = errorMessage;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithSpecialRequests(string specialRequests)
    {
        _creationData.SpecialRequests = specialRequests;
        return this;
    }
    
    public FakeWorldBuildingPhaseBuilder WithAttemptNumber(int attemptNumber)
    {
        _creationData.AttemptNumber = attemptNumber;
        return this;
    }
    
    public WorldBuildingPhase Build()
    {
        var result = WorldBuildingPhase.Create(_creationData);
        return result;
    }
}