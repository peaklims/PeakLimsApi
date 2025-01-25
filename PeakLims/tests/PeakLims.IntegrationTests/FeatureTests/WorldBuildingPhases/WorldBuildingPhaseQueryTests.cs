namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingPhases;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;
using PeakLims.Domain.WorldBuildingPhases.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class WorldBuildingPhaseQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_worldbuildingphase_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingPhaseOne = new FakeWorldBuildingPhaseBuilder().Build();
        await testingServiceScope.InsertAsync(worldBuildingPhaseOne);

        // Act
        var query = new GetWorldBuildingPhase.Query(worldBuildingPhaseOne.Id);
        var worldBuildingPhase = await testingServiceScope.SendAsync(query);

        // Assert
        worldBuildingPhase.ResultData.Should().Be(worldBuildingPhaseOne.ResultData);
        worldBuildingPhase.StartedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.StartedAt, 1.Seconds());
        worldBuildingPhase.EndedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseOne.EndedAt, 1.Seconds());
        worldBuildingPhase.StepNumber.Should().Be(worldBuildingPhaseOne.StepNumber);
        worldBuildingPhase.Comments.Should().Be(worldBuildingPhaseOne.Comments);
        worldBuildingPhase.ErrorMessage.Should().Be(worldBuildingPhaseOne.ErrorMessage);
        worldBuildingPhase.SpecialRequests.Should().Be(worldBuildingPhaseOne.SpecialRequests);
        worldBuildingPhase.AttemptNumber.Should().Be(worldBuildingPhaseOne.AttemptNumber);
        worldBuildingPhase.Name.Should().Be(worldBuildingPhaseOne.Name.Value);
        worldBuildingPhase.Status.Should().Be(worldBuildingPhaseOne.Status.Value);
    }

    [Fact]
    public async Task get_worldbuildingphase_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetWorldBuildingPhase.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}