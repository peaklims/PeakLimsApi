namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using AutoBogus;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Models;

public sealed class FakeAccessionForUpdate : AutoFaker<AccessionForUpdate>
{
    public FakeAccessionForUpdate()
    {
    }
}