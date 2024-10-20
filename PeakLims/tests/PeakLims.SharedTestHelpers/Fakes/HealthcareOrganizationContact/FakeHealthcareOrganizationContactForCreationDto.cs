namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;

using AutoBogus;
using Domain.Npis;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;

public sealed class FakeHealthcareOrganizationContactForCreationDto : AutoFaker<HealthcareOrganizationContactForCreationDto>
{
    public FakeHealthcareOrganizationContactForCreationDto()
    {
        RuleFor(x => x.Npi, _ => NPI.Random());
    }
}