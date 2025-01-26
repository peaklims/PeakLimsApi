namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingAttempts;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PeakLims.Domain.WorldBuildingAttempts.Features;

public class AddWorldBuildingAttemptCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_worldbuildingattempt_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingAttemptOne = new FakeWorldBuildingAttemptForCreationDto().Generate();

        // Act
        var command = new AddWorldBuildingAttempt.Command(worldBuildingAttemptOne);
        var worldBuildingAttemptReturned = await testingServiceScope.SendAsync(command);
        var worldBuildingAttemptCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.WorldBuildingAttempts
            .FirstOrDefaultAsync(w => w.Id == worldBuildingAttemptReturned.Id));

        // Assert
        worldBuildingAttemptReturned.Status.Should().Be(worldBuildingAttemptOne.Status);

        worldBuildingAttemptCreated.Status.Value.Should().Be(worldBuildingAttemptOne.Status);
    }
}