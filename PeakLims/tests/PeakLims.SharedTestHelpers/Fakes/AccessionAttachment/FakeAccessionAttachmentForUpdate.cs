namespace PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;

using AutoBogus;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.Models;

public sealed class FakeAccessionAttachmentForUpdate : AutoFaker<AccessionAttachmentForUpdate>
{
    public FakeAccessionAttachmentForUpdate()
    {
        RuleFor(x => x.Type, f => f.PickRandom(AccessionAttachmentType.ListNames()));
    }
}