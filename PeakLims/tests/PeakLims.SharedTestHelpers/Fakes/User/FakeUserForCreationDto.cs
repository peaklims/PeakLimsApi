namespace PeakLims.SharedTestHelpers.Fakes.User;

using AutoBogus;
using PeakLims.Domain;
using PeakLims.Domain.Users.Dtos;
using PeakLims.Domain.Roles;
using PeakLims.Domain.Users.Models;

public sealed class FakeUserForCreationDto : AutoFaker<UserForCreationDto>
{
    public FakeUserForCreationDto()
    {
        RuleFor(u => u.Email, f => f.Person.Email);
    }
}