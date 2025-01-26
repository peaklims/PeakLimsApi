namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingAttempts;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;
using PeakLims.Domain.WorldBuildingAttempts.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class WorldBuildingAttemptQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_worldbuildingattempt_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingAttemptOne = new FakeWorldBuildingAttemptBuilder().Build();
        await testingServiceScope.InsertAsync(worldBuildingAttemptOne);

        // Act
        var query = new GetWorldBuildingAttempt.Query(worldBuildingAttemptOne.Id);
        var worldBuildingAttempt = await testingServiceScope.SendAsync(query);

        // Assert
        worldBuildingAttempt.Status.Should().Be(worldBuildingAttemptOne.Status.Value);
    }

    [Fact]
    public async Task get_worldbuildingattempt_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetWorldBuildingAttempt.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}