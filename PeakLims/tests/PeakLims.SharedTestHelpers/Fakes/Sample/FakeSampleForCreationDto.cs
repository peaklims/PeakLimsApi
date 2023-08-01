namespace PeakLims.SharedTestHelpers.Fakes.Sample;

using AutoBogus;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Dtos;

public sealed class FakeSampleForCreationDto : AutoFaker<SampleForCreationDto>
{
    public FakeSampleForCreationDto()
    {
    }
}