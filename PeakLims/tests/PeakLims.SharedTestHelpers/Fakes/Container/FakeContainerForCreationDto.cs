namespace PeakLims.SharedTestHelpers.Fakes.Container;

using AutoBogus;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.Dtos;

public sealed class FakeContainerForCreationDto : AutoFaker<ContainerForCreationDto>
{
    public FakeContainerForCreationDto()
    {
    }
}