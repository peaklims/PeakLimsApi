namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;

using AutoBogus;
using Domain.Npis;
using Npi;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;

public sealed class FakeHealthcareOrganizationContactForUpdate : AutoFaker<HealthcareOrganizationContactForUpdate>
{
    public FakeHealthcareOrganizationContactForUpdate()
    {
        RuleFor(x => x.Npi, _ => NPI.Of(NpiGenerator.GenerateRandomNpi()));
    }
}