namespace PeakLims.SharedTestHelpers.Fakes.AccessionContact;

using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Models;

public class FakeAccessionContactBuilder
{
    private AccessionContactForCreation _creationData = new FakeAccessionContactForCreation().Generate();

    public FakeAccessionContactBuilder WithModel(AccessionContactForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeAccessionContactBuilder WithTargetType(string targetType)
    {
        _creationData.TargetType = targetType;
        return this;
    }
    
    public FakeAccessionContactBuilder WithTargetValue(string targetValue)
    {
        _creationData.TargetValue = targetValue;
        return this;
    }
    
    public AccessionContact Build()
    {
        var result = AccessionContact.Create(_creationData);
        return result;
    }
}