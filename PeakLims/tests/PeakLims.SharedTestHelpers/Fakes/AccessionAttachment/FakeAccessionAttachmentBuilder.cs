namespace PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;

using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.Models;

public class FakeAccessionAttachmentBuilder
{
    private AccessionAttachmentForCreation _creationData = new FakeAccessionAttachmentForCreation().Generate();

    public FakeAccessionAttachmentBuilder WithModel(AccessionAttachmentForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeAccessionAttachmentBuilder WithAccessionAttachmentType(string accessionAttachmentType)
    {
        _creationData.Type = accessionAttachmentType;
        return this;
    }
    
    public FakeAccessionAttachmentBuilder WithComments(string comments)
    {
        _creationData.Comments = comments;
        return this;
    }
    
    public AccessionAttachment Build()
    {
        var result = AccessionAttachment.Create(_creationData);
        return result;
    }
}