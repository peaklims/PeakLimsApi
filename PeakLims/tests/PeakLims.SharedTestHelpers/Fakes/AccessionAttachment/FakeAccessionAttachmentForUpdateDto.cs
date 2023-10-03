namespace PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;

using AutoBogus;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.Dtos;

public sealed class FakeAccessionAttachmentForUpdateDto : AutoFaker<AccessionAttachmentForUpdateDto>
{
    public FakeAccessionAttachmentForUpdateDto()
    {
        RuleFor(x => x.Type, f => f.PickRandom(AccessionAttachmentType.ListNames()));
    }
}