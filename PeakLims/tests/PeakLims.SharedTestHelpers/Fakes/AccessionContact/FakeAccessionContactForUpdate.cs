namespace PeakLims.SharedTestHelpers.Fakes.AccessionContact;

using AutoBogus;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Models;

public sealed class FakeAccessionContactForUpdate : AutoFaker<AccessionContactForUpdate>
{
    public FakeAccessionContactForUpdate()
    {
        RuleFor(a => a.TargetType, f => f.PickRandom<TargetTypeEnum>(TargetTypeEnum.List).Name);
    }
}