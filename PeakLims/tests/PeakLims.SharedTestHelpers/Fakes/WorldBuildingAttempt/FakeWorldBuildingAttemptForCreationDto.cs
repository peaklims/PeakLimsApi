namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;

using AutoBogus;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Domain.WorldBuildingStatuses;

public sealed class FakeWorldBuildingAttemptForCreationDto : AutoFaker<WorldBuildingAttemptForCreationDto>
{
    public FakeWorldBuildingAttemptForCreationDto()
    {
        RuleFor(w => w.Status, f => f.PickRandom(WorldBuildingStatus.ListNames()));
    }
}