namespace PeakLims.SharedTestHelpers.Fakes.Test;

using AutoBogus;
using PeakLims.Domain.Tests;
using PeakLims.Domain.Tests.Models;

public sealed class FakeTestForCreation : AutoFaker<TestForCreation>
{
    public FakeTestForCreation()
    {
    }
}