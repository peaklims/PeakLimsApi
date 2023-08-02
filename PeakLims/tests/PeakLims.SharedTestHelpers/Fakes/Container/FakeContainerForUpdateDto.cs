namespace PeakLims.SharedTestHelpers.Fakes.Container;

using AutoBogus;
using Domain.SampleTypes;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;

public sealed class FakeContainerForUpdateDto : AutoFaker<ContainerForUpdateDto>
{
    public FakeContainerForUpdateDto()
    {
        RuleFor(x => x.UsedFor, f => f.PickRandom(SampleType.ListNames()));
    }
}