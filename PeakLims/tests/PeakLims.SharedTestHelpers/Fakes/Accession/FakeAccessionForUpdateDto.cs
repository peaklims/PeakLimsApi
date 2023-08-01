namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using AutoBogus;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;

public sealed class FakeAccessionForUpdateDto : AutoFaker<AccessionForUpdateDto>
{
    public FakeAccessionForUpdateDto()
    {
    }
}