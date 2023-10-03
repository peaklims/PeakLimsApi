namespace PeakLims.SharedTestHelpers.Fakes.AccessionAttachment;

using AutoBogus;
using PeakLims.Domain.AccessionAttachments;
using PeakLims.Domain.AccessionAttachments.Models;

public sealed class FakeAccessionAttachmentForCreation : AutoFaker<AccessionAttachmentForCreation>
{
    public FakeAccessionAttachmentForCreation()
    {
        RuleFor(x => x.Type, f => f.PickRandom(AccessionAttachmentType.ListNames()));
    }
}