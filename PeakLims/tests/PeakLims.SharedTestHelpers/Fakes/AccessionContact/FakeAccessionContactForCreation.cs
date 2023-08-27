namespace PeakLims.SharedTestHelpers.Fakes.AccessionContact;

using AutoBogus;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Models;

public sealed class FakeAccessionContactForCreation : AutoFaker<AccessionContactForCreation>
{
    public FakeAccessionContactForCreation()
    {
        RuleFor(a => a.TargetType, f => f.PickRandom<TargetTypeEnum>(TargetTypeEnum.List).Name);
    }
}