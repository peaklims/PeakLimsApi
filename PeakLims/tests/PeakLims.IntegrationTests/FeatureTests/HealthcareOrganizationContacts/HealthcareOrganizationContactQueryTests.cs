namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.HealthcareOrganization;

public class HealthcareOrganizationContactQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_healthcareorganizationcontact_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var healthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        org.AddContact(healthcareOrganizationContact);
        await testingServiceScope.InsertAsync(org);

        // Act
        var query = new GetHealthcareOrganizationContact.Query(healthcareOrganizationContact.Id);
        var healthcareOrganizationContactResponse = await testingServiceScope.SendAsync(query);

        // Assert
        healthcareOrganizationContactResponse.FirstName.Should().Be(healthcareOrganizationContact.FirstName);
        healthcareOrganizationContactResponse.LastName.Should().Be(healthcareOrganizationContact.LastName);
        healthcareOrganizationContactResponse.Email.Should().Be(healthcareOrganizationContact.Email);
        healthcareOrganizationContactResponse.Npi.Should().Be(healthcareOrganizationContact.Npi);
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

    [Fact(Skip = "need to redo permission granularity")]
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