namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using SharedKernel.Exceptions;

public class AddHealthcareOrganizationContactCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_healthcareorganizationcontact_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactForCreationDto().Generate();

        // Act
        var command = new AddHealthcareOrganizationContact.Command(fakeHealthcareOrganizationContactOne);
        var healthcareOrganizationContactReturned = await testingServiceScope.SendAsync(command);
        var healthcareOrganizationContactCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .FirstOrDefaultAsync(h => h.Id == healthcareOrganizationContactReturned.Id));

        // Assert
        healthcareOrganizationContactReturned.Name.Should().Be(fakeHealthcareOrganizationContactOne.Name);
        healthcareOrganizationContactReturned.Email.Should().Be(fakeHealthcareOrganizationContactOne.Email);
        healthcareOrganizationContactReturned.Npi.Should().Be(fakeHealthcareOrganizationContactOne.Npi);

        healthcareOrganizationContactCreated.Name.Should().Be(fakeHealthcareOrganizationContactOne.Name);
        healthcareOrganizationContactCreated.Email.Should().Be(fakeHealthcareOrganizationContactOne.Email);
        healthcareOrganizationContactCreated.Npi.Should().Be(fakeHealthcareOrganizationContactOne.Npi);
    }

    [Fact]
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