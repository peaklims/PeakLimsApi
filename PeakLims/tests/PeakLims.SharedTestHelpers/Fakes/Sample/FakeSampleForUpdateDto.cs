namespace PeakLims.SharedTestHelpers.Fakes.Sample;

using AutoBogus;
using Domain.SampleTypes;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;

public sealed class FakeSampleForUpdateDto : AutoFaker<SampleForUpdateDto>
{
    public FakeSampleForUpdateDto()
    {
        RuleFor(x => x.Type, f => f.PickRandom(SampleType.ListNames()));
        RuleFor(x => x.ContainerId, _ => null);
    }
}