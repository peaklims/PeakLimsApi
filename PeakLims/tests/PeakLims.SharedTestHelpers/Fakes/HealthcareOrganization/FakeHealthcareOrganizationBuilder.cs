namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;

using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Models;

public class FakeHealthcareOrganizationBuilder
{
    private HealthcareOrganizationForCreation _creationData = new FakeHealthcareOrganizationForCreation().Generate();

    public FakeHealthcareOrganizationBuilder WithModel(HealthcareOrganizationForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeHealthcareOrganizationBuilder WithName(string name)
    {
        _creationData.Name = name;
        return this;
    }
    
    public FakeHealthcareOrganizationBuilder WithEmail(string email)
    {
        _creationData.Email = email;
        return this;
    }
    
    public HealthcareOrganization Build()
    {
        var result = HealthcareOrganization.Create(_creationData);
        return result;
    }
}