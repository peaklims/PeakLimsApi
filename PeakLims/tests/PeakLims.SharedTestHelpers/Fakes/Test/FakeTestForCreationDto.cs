namespace PeakLims.SharedTestHelpers.Fakes.Test;

using AutoBogus;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Dtos;

public sealed class FakeTestForCreationDto : AutoFaker<TestForCreationDto>
{
    public FakeTestForCreationDto()
    {
    }
}