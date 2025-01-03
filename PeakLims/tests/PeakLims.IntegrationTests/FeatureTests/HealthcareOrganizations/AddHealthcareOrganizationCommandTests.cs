namespace PeakLims.IntegrationTests.FeatureTests.HealthcareOrganizations;

using PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using Exceptions;
using PeakLims.Domain.HealthcareOrganizations.Features;

public class AddHealthcareOrganizationCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_healthcareorganization_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationForCreationDto().Generate();

        // Act
        var command = new AddHealthcareOrganization.Command(fakeHealthcareOrganizationOne);
        var healthcareOrganizationReturned = await testingServiceScope.SendAsync(command);
        var healthcareOrganizationCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.HealthcareOrganizations
            .FirstOrDefaultAsync(h => h.Id == healthcareOrganizationReturned.Id));

        // Assert
        healthcareOrganizationReturned.Name.Should().Be(fakeHealthcareOrganizationOne.Name);

        healthcareOrganizationCreated.Name.Should().Be(fakeHealthcareOrganizationOne.Name);
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddHealthcareOrganizations);
        var fakeHealthcareOrganizationOne = new FakeHealthcareOrganizationForCreationDto();

        // Act
        var command = new AddHealthcareOrganization.Command(fakeHealthcareOrganizationOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}