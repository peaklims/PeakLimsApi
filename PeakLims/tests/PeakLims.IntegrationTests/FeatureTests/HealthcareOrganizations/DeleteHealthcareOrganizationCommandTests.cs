namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.Domain.HealthcareOrganizations.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteHealthcareOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_healthcareorganization_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var healthcareOrganization = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationOne.Id));

        // Act
        var command = new DeleteHealthcareOrganization.Command(healthcareOrganization.Id);
        await testingServiceScope.SendAsync(command);
        var healthcareOrganizationResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations.CountAsync(h => h.Id == healthcareOrganization.Id));

        // Assert
        healthcareOrganizationResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_healthcareorganization_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteHealthcareOrganization.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_healthcareorganization_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);
        var healthcareOrganization = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationOne.Id));

        // Act
        var command = new DeleteHealthcareOrganization.Command(healthcareOrganization.Id);
        await testingServiceScope.SendAsync(command);
        var deletedHealthcareOrganization = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == healthcareOrganization.Id));

        // Assert
        deletedHealthcareOrganization?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteHealthcareOrganizations);

        // Act
        var command = new DeleteHealthcareOrganization.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}