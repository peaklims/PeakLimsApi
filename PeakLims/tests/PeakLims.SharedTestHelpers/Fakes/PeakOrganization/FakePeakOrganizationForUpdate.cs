namespace PeakLims.SharedTestHelpers.Fakes.PeakOrganization;

using AutoBogus;
using PeakLims.Domain.PeakOrganizations;
using PeakLims.Domain.PeakOrganizations.Models;

public sealed class FakePeakOrganizationForUpdate : AutoFaker<PeakOrganizationForUpdate>
{
    public FakePeakOrganizationForUpdate()
    {
        RuleFor(x => x.Name, f => f.Company.CompanyName());
    }
}