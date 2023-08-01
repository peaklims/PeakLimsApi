namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizationContacts;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganizationContact;
using PeakLims.Domain.HealthcareOrganizationContacts.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteHealthcareOrganizationContactCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_healthcareorganizationcontact_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationContactOne);
        var healthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationContactOne.Id));

        // Act
        var command = new DeleteHealthcareOrganizationContact.Command(healthcareOrganizationContact.Id);
        await testingServiceScope.SendAsync(command);
        var healthcareOrganizationContactResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts.CountAsync(h => h.Id == healthcareOrganizationContact.Id));

        // Assert
        healthcareOrganizationContactResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_healthcareorganizationcontact_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteHealthcareOrganizationContact.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_healthcareorganizationcontact_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationContactOne = new FakeHealthcareOrganizationContactBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationContactOne);
        var healthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationContactOne.Id));

        // Act
        var command = new DeleteHealthcareOrganizationContact.Command(healthcareOrganizationContact.Id);
        await testingServiceScope.SendAsync(command);
        var deletedHealthcareOrganizationContact = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizationContacts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == healthcareOrganizationContact.Id));

        // Assert
        deletedHealthcareOrganizationContact?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteHealthcareOrganizationContacts);

        // Act
        var command = new DeleteHealthcareOrganizationContact.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}