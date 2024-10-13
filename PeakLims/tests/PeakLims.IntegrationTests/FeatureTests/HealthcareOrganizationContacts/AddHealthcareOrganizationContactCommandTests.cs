namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using SharedTestHelpers.Fakes.HealthcareOrganization;

public class AddHealthcareOrganizationContactCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_healthcareorganizationcontact_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var healthcareOrganization = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(healthcareOrganization);
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactForCreationDto()
            .RuleFor(x => x.HealthcareOrganizationId, healthcareOrganization.Id)
            .Generate();

        // Act
        var command = new AddHealthcareOrganizationContact.Command(fakeHealthcareOrganizationContactOne);
        var healthcareOrganizationContactReturned = await testingServiceScope.SendAsync(command);
        var healthcareOrganizationContactCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .Include(x => x.HealthcareOrganization)
            .FirstOrDefaultAsync(h => h.Id == healthcareOrganizationContactReturned.Id));

        // Assert
        healthcareOrganizationContactReturned.FirstName.Should().Be(fakeHealthcareOrganizationContactOne.FirstName);
        healthcareOrganizationContactReturned.LastName.Should().Be(fakeHealthcareOrganizationContactOne.LastName);
        healthcareOrganizationContactReturned.Email.Should().Be(fakeHealthcareOrganizationContactOne.Email);
        healthcareOrganizationContactReturned.Npi.Should().Be(fakeHealthcareOrganizationContactOne.Npi);
        healthcareOrganizationContactReturned.HealthcareOrganizationId.Should().Be(fakeHealthcareOrganizationContactOne.HealthcareOrganizationId);

        healthcareOrganizationContactCreated.FirstName.Should().Be(fakeHealthcareOrganizationContactOne.FirstName);
        healthcareOrganizationContactCreated.LastName.Should().Be(fakeHealthcareOrganizationContactOne.LastName);
        healthcareOrganizationContactCreated.Email.Should().Be(fakeHealthcareOrganizationContactOne.Email);
        healthcareOrganizationContactCreated.Npi.Value.Should().Be(fakeHealthcareOrganizationContactOne.Npi);
        healthcareOrganizationContactCreated.HealthcareOrganization.Id.Should().Be(fakeHealthcareOrganizationContactOne.HealthcareOrganizationId);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddHealthcareOrganizationContacts);
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactForCreationDto();

        // Act
        var command = new AddHealthcareOrganizationContact.Command(fakeHealthcareOrganizationContactOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}