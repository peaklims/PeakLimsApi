namespace PeakLims.SharedTestHelpers.Fakes.Sample;

using AutoBogus;
using Domain.SampleTypes;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;

public sealed class FakeSampleForCreation : AutoFaker<SampleForCreation>
{
    public FakeSampleForCreation()
    {
        RuleFor(x => x.Type, f => f.PickRandom(SampleType.ListNames()));
    }
}