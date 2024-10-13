namespace PeakLims.SharedTestHelpers.Fakes.PeakOrganization;

using AutoBogus;
using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.Dtos;

public sealed class FakePeakOrganizationForUpdateDto : AutoFaker<PeakOrganizationForUpdateDto>
{
    public FakePeakOrganizationForUpdateDto()
    {
        RuleFor(x => x.Name, f => f.Company.CompanyName());
    }
}