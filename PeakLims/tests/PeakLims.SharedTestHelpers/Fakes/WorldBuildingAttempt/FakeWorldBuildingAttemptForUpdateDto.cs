namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;

using AutoBogus;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Domain.WorldBuildingStatuses;

public sealed class FakeWorldBuildingAttemptForUpdateDto : AutoFaker<WorldBuildingAttemptForUpdateDto>
{
    public FakeWorldBuildingAttemptForUpdateDto()
    {
        RuleFor(w => w.Status, f => f.PickRandom(WorldBuildingStatus.ListNames()));
    }
}