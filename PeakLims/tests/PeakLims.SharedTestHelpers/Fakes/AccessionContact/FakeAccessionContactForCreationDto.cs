namespace PeakLims.SharedTestHelpers.Fakes.AccessionContact;

using AutoBogus;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Dtos;

public sealed class FakeAccessionContactForCreationDto : AutoFaker<AccessionContactForCreationDto>
{
    public FakeAccessionContactForCreationDto()
    {
        RuleFor(a => a.TargetType, f => f.PickRandom<TargetTypeEnum>(TargetTypeEnum.List).Name);
    }
}