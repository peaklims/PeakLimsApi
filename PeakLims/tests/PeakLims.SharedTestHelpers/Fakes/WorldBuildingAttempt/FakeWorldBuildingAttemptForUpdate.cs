namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;

using AutoBogus;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingStatuses;

public sealed class FakeWorldBuildingAttemptForUpdate : AutoFaker<WorldBuildingAttemptForUpdate>
{
    public FakeWorldBuildingAttemptForUpdate()
    {
        RuleFor(w => w.Status, f => f.PickRandom(WorldBuildingStatus.ListNames()));
    }
}