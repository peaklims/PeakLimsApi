namespace PeakLims.SharedTestHelpers.Fakes.AccessionComment;

using AutoBogus;
using PeakLims.Domain.AccessionComments;
using PeakLims.Domain.AccessionComments.Dtos;

public sealed class FakeAccessionCommentForCreationDto : AutoFaker<AccessionCommentForCreationDto>
{
    public FakeAccessionCommentForCreationDto()
    {
    }
}