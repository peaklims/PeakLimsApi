namespace PeakLims.SharedTestHelpers.Fakes.Sample;

using AutoBogus;
using PeakLims.Domain.Samples;
using PeakLims.Domain.Samples.Models;

public sealed class FakeSampleForCreation : AutoFaker<SampleForCreation>
{
    public FakeSampleForCreation()
    {
    }
}