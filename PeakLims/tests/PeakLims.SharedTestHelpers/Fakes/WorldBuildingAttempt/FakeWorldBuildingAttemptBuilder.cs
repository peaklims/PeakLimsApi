namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;

using PeakLims.Domain.WorldBuildingAttempts;

public class FakeWorldBuildingAttemptBuilder
{
    private WorldBuildingAttemptForCreation _creationData = new FakeWorldBuildingAttemptForCreation().Generate();

    public FakeWorldBuildingAttemptBuilder WithModel(WorldBuildingAttemptForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public WorldBuildingAttempt Build()
    {
        var result = WorldBuildingAttempt.CreateStandardWorld(_creationData);
        return result;
    }
}