namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;

public class UpdateHealthcareOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_healthcareorganization_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationBuilder().Build();
        var updatedHealthcareOrganizationDto = new FakeHealthcareOrganizationForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeHealthcareOrganizationOne);

        var healthcareOrganization = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations
            .FirstOrDefaultAsync(h => h.Id == fakeHealthcareOrganizationOne.Id));

        // Act
        var command = new UpdateHealthcareOrganization.Command(healthcareOrganization.Id, updatedHealthcareOrganizationDto);
        await testingServiceScope.SendAsync(command);
        var updatedHealthcareOrganization = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations.FirstOrDefaultAsync(h => h.Id == healthcareOrganization.Id));

        // Assert
        updatedHealthcareOrganization.Name.Should().Be(updatedHealthcareOrganizationDto.Name);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateHealthcareOrganizations);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationForUpdateDto();

        // Act
        var command = new UpdateHealthcareOrganization.Command(Guid.NewGuid(), fakeHealthcareOrganizationOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}