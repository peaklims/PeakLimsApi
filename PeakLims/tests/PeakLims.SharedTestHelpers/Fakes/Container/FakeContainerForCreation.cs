namespace PeakLims.SharedTestHelpers.Fakes.Container;

using AutoBogus;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Models;

public sealed class FakeContainerForCreation : AutoFaker<ContainerForCreation>
{
    public FakeContainerForCreation()
    {
    }
}