namespace PeakLims.SharedTestHelpers.Fakes.AccessionComment;

using AutoBogus;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Models;

public sealed class FakeAccessionCommentForCreation : AutoFaker<AccessionCommentForCreation>
{
    public FakeAccessionCommentForCreation()
    {
    }
}