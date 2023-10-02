namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;

public class HealthcareOrganizationContactQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_healthcareorganizationcontact_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationContactOne);

        // Act
        var query = new GetHealthcareOrganizationContact.Query(fakeHealthcareOrganizationContactOne.Id);
        var healthcareOrganizationContact = await testingServiceScope.SendAsync(query);

        // Assert
        healthcareOrganizationContact.FirstName.Should().Be(fakeHealthcareOrganizationContactOne.FirstName);
        healthcareOrganizationContact.LastName.Should().Be(fakeHealthcareOrganizationContactOne.LastName);
        healthcareOrganizationContact.Email.Should().Be(fakeHealthcareOrganizationContactOne.Email);
        healthcareOrganizationContact.Npi.Should().Be(fakeHealthcareOrganizationContactOne.Npi);
    }

    [Fact]
    public async Task get_healthcareorganizationcontact_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetHealthcareOrganizationContact.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadHealthcareOrganizationContacts);

        // Act
        var command = new GetHealthcareOrganizationContact.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}