namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingPhases;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PeakLims.Domain.WorldBuildingPhases.Features;

public class AddWorldBuildingPhaseCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_worldbuildingphase_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingPhaseOne = new FakeWorldBuildingPhaseForCreationDto().Generate();

        // Act
        var command = new AddWorldBuildingPhase.Command(worldBuildingPhaseOne);
        var worldBuildingPhaseReturned = await testingServiceScope.SendAsync(command);
        var worldBuildingPhaseCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.WorldBuildingPhases
            .FirstOrDefaultAsync(w => w.Id == worldBuildingPhaseReturned.Id));

        // Assert
        worldBuildingPhaseReturned.ResultData.Should().Be(worldBuildingPhaseOne.ResultData);
        worldBuildingPhaseReturned.StartedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.StartedAt, 1.Seconds());
        worldBuildingPhaseReturned.EndedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.EndedAt, 1.Seconds());
        worldBuildingPhaseReturned.StepNumber.Should().Be(worldBuildingPhaseOne.StepNumber);
        worldBuildingPhaseReturned.Comments.Should().Be(worldBuildingPhaseOne.Comments);
        worldBuildingPhaseReturned.ErrorMessage.Should().Be(worldBuildingPhaseOne.ErrorMessage);
        worldBuildingPhaseReturned.SpecialRequests.Should().Be(worldBuildingPhaseOne.SpecialRequests);
        worldBuildingPhaseReturned.AttemptNumber.Should().Be(worldBuildingPhaseOne.AttemptNumber);
        worldBuildingPhaseReturned.Name.Should().Be(worldBuildingPhaseOne.Name);
        worldBuildingPhaseReturned.Status.Should().Be(worldBuildingPhaseOne.Status);

        worldBuildingPhaseCreated.ResultData.Should().Be(worldBuildingPhaseOne.ResultData);
        worldBuildingPhaseCreated.StartedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.StartedAt, 1.Seconds());
        worldBuildingPhaseCreated.EndedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.EndedAt, 1.Seconds());
        worldBuildingPhaseCreated.StepNumber.Should().Be(worldBuildingPhaseOne.StepNumber);
        worldBuildingPhaseCreated.Comments.Should().Be(worldBuildingPhaseOne.Comments);
        worldBuildingPhaseCreated.ErrorMessage.Should().Be(worldBuildingPhaseOne.ErrorMessage);
        worldBuildingPhaseCreated.SpecialRequests.Should().Be(worldBuildingPhaseOne.SpecialRequests);
        worldBuildingPhaseCreated.AttemptNumber.Should().Be(worldBuildingPhaseOne.AttemptNumber);
        worldBuildingPhaseCreated.Name.Value.Should().Be(worldBuildingPhaseOne.Name);
        worldBuildingPhaseCreated.Status.Value.Should().Be(worldBuildingPhaseOne.Status);
    }
}