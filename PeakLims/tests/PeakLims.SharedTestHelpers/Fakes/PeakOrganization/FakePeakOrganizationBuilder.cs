namespace PeakLims.SharedTestHelpers.Fakes.PeakOrganization;

using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.Models;

public class FakePeakOrganizationBuilder
{
    private PeakOrganizationForCreation _creationData = new FakePeakOrganizationForCreation().Generate();

    public FakePeakOrganizationBuilder WithModel(PeakOrganizationForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakePeakOrganizationBuilder WithName(string name)
    {
        _creationData.Name = name;
        return this;
    }
    
    public PeakOrganization Build()
    {
        var result = PeakOrganization.Create(_creationData);
        return result;
    }
}