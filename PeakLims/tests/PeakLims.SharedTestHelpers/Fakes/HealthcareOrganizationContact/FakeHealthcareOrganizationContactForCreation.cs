namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;

using AutoBogus;
using Domain.Npis;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;

public sealed class FakeHealthcareOrganizationContactForCreation : AutoFaker<HealthcareOrganizationContactForCreation>
{
    public FakeHealthcareOrganizationContactForCreation()
    {
        RuleFor(x => x.Npi, _ => NPI.Random());
    }
}