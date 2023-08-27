namespace PeakLims.SharedTestHelpers.Fakes.AccessionContact;

using Domain.HealthcareOrganizationContacts;
using HealthcareOrganizationContact;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Models;

public class FakeAccessionContactBuilder
{
    private HealthcareOrganizationContact _creationData = new FakeHealthcareOrganizationContactBuilder().Build();

    
    // public FakeAccessionContactBuilder WithTargetType(string targetType)
    // {
    //     _creationData.TargetType = targetType;
    //     return this;
    // }
    //
    // public FakeAccessionContactBuilder WithTargetValue(string targetValue)
    // {
    //     _creationData.TargetValue = targetValue;
    //     return this;
    // }
    
    public AccessionContact Build()
    {
        var result = AccessionContact.Create(_creationData);
        return result;
    }
}