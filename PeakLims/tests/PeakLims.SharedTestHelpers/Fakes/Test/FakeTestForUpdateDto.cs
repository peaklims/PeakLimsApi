namespace PeakLims.SharedTestHelpers.Fakes.Test;

using AutoBogus;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Dtos;

public sealed class FakeTestForUpdateDto : AutoFaker<TestForUpdateDto>
{
    public FakeTestForUpdateDto()
    {
        RuleFor(x => x.TurnAroundTime, x => x.Random.Int(min: 1, max: 1000));
    }
}