namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using SharedTestHelpers.Fakes.HealthcareOrganization;

public class UpdateHealthcareOrganizationContactCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_healthcareorganizationcontact_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var org = new FakeHealthcareOrganizationBuilder().Build();
        var healthcareOrganizationContact = new FakeHealthcareOrganizationContactBuilder().Build();
        org.AddContact(healthcareOrganizationContact);
        await testingServiceScope.InsertAsync(org);
        var updatedHealthcareOrganizationContactDto = new FakeHealthcareOrganizationContactForUpdateDto().Generate();

        // Act
        var command = new UpdateHealthcareOrganizationContact.Command(healthcareOrganizationContact.Id, updatedHealthcareOrganizationContactDto);
        await testingServiceScope.SendAsync(command);
        var updatedHealthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts.FirstOrDefaultAsync(h => h.Id == healthcareOrganizationContact.Id));

        // Assert
        updatedHealthcareOrganizationContact.FirstName.Should().Be(updatedHealthcareOrganizationContactDto.FirstName);
        updatedHealthcareOrganizationContact.LastName.Should().Be(updatedHealthcareOrganizationContactDto.LastName);
        updatedHealthcareOrganizationContact.Email.Should().Be(updatedHealthcareOrganizationContactDto.Email);
        updatedHealthcareOrganizationContact.Npi.Value.Should().Be(updatedHealthcareOrganizationContactDto.Npi);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateHealthcareOrganizationContacts);
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactForUpdateDto();

        // Act
        var command = new UpdateHealthcareOrganizationContact.Command(Guid.NewGuid(), fakeHealthcareOrganizationContactOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}