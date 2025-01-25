namespace PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;

using AutoBogus;
using Domain.WorldBuildingPhaseNames;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Domain.WorldBuildingStatuses;

public sealed class FakeWorldBuildingPhaseForUpdate : AutoFaker<WorldBuildingPhaseForUpdate>
{
    public FakeWorldBuildingPhaseForUpdate()
    {
        RuleFor(w => w.Name, f => f.PickRandom(WorldBuildingPhaseName.ListNames()));
        RuleFor(w => w.Status, f => f.PickRandom(WorldBuildingStatus.ListNames()));
    }
}