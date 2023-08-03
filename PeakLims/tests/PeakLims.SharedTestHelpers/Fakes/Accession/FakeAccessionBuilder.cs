namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using PeakLims.Domain.Accessions;

public class FakeAccessionBuilder
{
    public Accession Build()
    {
        var result = Accession.Create();
        return result;
    }
}