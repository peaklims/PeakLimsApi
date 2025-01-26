namespace PeakLims.SharedTestHelpers.Fakes.User;

using AutoBogus;
using PeakLims.Domain;
using PeakLims.Domain.Users.Dtos;
using PeakLims.Domain.Roles;
using PeakLims.Domain.Users.Models;
using Utilities;

public sealed class FakeUserForCreation : AutoFaker<UserForCreation>
{
    public FakeUserForCreation()
    {
        RuleFor(u => u.Email, f => f.Person.Email);
        RuleFor(x => x.OrganizationId, TestingConsts.DefaultTestingOrganizationId);
    }
}