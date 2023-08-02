namespace PeakLims.SharedTestHelpers.Fakes.Container;

using AutoBogus;
using Domain.SampleTypes;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Models;

public sealed class FakeContainerForCreation : AutoFaker<ContainerForCreation>
{
    public FakeContainerForCreation()
    {
        RuleFor(x => x.UsedFor, f => f.PickRandom(SampleType.ListNames()));
    }
}