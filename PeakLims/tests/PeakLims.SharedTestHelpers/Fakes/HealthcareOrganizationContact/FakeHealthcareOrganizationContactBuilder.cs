namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;

using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;

public class FakeHealthcareOrganizationContactBuilder
{
    private HealthcareOrganizationContactForCreation _creationData = new FakeHealthcareOrganizationContactForCreation().Generate();

    public FakeHealthcareOrganizationContactBuilder WithModel(HealthcareOrganizationContactForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeHealthcareOrganizationContactBuilder WithFirstName(string name)
    {
        _creationData.FirstName = name;
        return this;
    }
    
    public FakeHealthcareOrganizationContactBuilder WithLastName(string name)
    {
        _creationData.LastName = name;
        return this;
    }
    
    public FakeHealthcareOrganizationContactBuilder WithEmail(string email)
    {
        _creationData.Email = email;
        return this;
    }
    
    public FakeHealthcareOrganizationContactBuilder WithNpi(string npi)
    {
        _creationData.Npi = npi;
        return this;
    }
    
    public HealthcareOrganizationContact Build()
    {
        var result = HealthcareOrganizationContact.Create(_creationData);
        return result;
    }
}