namespace PeakLims.SharedTestHelpers.Fakes.AccessionComment;

using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Models;

public class FakeAccessionCommentBuilder
{
    private AccessionCommentForCreation _creationData = new FakeAccessionCommentForCreation().Generate();

    public FakeAccessionCommentBuilder WithModel(AccessionCommentForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeAccessionCommentBuilder WithComment(string comment)
    {
        _creationData.Comment = comment;
        return this;
    }
    
    public FakeAccessionCommentBuilder WithStatus(string status)
    {
        _creationData.Status = status;
        return this;
    }
    
    public AccessionComment Build()
    {
        var result = AccessionComment.Create(_creationData);
        return result;
    }
}