namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;

using AutoBogus;
using Domain.Npis;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

public sealed class FakeHealthcareOrganizationContactForUpdateDto : AutoFaker<HealthcareOrganizationContactForUpdateDto>
{
    public FakeHealthcareOrganizationContactForUpdateDto()
    {
        RuleFor(x => x.Npi, _ => NPI.Random());
    }
}