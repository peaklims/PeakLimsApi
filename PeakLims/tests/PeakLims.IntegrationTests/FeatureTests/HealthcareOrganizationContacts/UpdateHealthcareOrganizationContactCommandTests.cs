namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateHealthcareOrganizationContactCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_healthcareorganizationcontact_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        var updatedHealthcareOrganizationContactDto = new FakeHealthcareOrganizationContactForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationContactOne);

        var healthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationContactOne.Id));

        // Act
        var command = new UpdateHealthcareOrganizationContact.Command(healthcareOrganizationContact.Id, updatedHealthcareOrganizationContactDto);
        await testingServiceScope.SendAsync(command);
        var updatedHealthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts.FirstOrDefaultAsync(h => h.Id == healthcareOrganizationContact.Id));

        // Assert
        updatedHealthcareOrganizationContact.Name.Should().Be(updatedHealthcareOrganizationContactDto.Name);
        updatedHealthcareOrganizationContact.Email.Should().Be(updatedHealthcareOrganizationContactDto.Email);
        updatedHealthcareOrganizationContact.Npi.Should().Be(updatedHealthcareOrganizationContactDto.Npi);
    }

    [Fact]
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