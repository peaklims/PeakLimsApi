namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;

using AutoBogus;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Dtos;

public sealed class FakeHealthcareOrganizationForCreationDto : AutoFaker<HealthcareOrganizationForCreationDto>
{
    public FakeHealthcareOrganizationForCreationDto()
    {
    }
}