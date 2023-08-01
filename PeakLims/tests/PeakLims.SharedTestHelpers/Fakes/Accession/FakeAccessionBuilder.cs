namespace PeakLims.SharedTestHelpers.Fakes.Accession;

using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Models;

public class FakeAccessionBuilder
{
    private AccessionForCreation _creationData = new FakeAccessionForCreation().Generate();

    public FakeAccessionBuilder WithModel(AccessionForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeAccessionBuilder WithAccessionNumber(string accessionNumber)
    {
        _creationData.AccessionNumber = accessionNumber;
        return this;
    }
    
    public FakeAccessionBuilder WithStatus(string status)
    {
        _creationData.Status = status;
        return this;
    }
    
    public Accession Build()
    {
        var result = Accession.Create(_creationData);
        return result;
    }
}