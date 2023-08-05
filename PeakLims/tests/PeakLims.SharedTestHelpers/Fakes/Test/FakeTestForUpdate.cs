namespace PeakLims.SharedTestHelpers.Fakes.Test;

using AutoBogus;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;

public sealed class FakeTestForUpdate : AutoFaker<TestForUpdate>
{
    public FakeTestForUpdate()
    {
        RuleFor(x => x.TurnAroundTime, x => x.Random.Int(min: 1, max: 1000));
    }
}