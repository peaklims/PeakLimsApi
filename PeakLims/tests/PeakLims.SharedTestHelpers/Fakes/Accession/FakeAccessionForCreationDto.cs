namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using AutoBogus;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;

public sealed class FakeAccessionForCreationDto : AutoFaker<AccessionForCreationDto>
{
    public FakeAccessionForCreationDto()
    {
    }
}