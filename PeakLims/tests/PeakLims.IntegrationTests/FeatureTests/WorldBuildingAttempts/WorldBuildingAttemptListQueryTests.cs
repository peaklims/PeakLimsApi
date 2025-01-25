namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingAttempts;

using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;
using PeakLims.Domain.WorldBuildingAttempts.Features;
using Domain;
using System.Threading.Tasks;

public class WorldBuildingAttemptListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_worldbuildingattempt_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingAttemptOne = new FakeWorldBuildingAttemptBuilder().Build();
        var worldBuildingAttemptTwo = new FakeWorldBuildingAttemptBuilder().Build();
        var queryParameters = new WorldBuildingAttemptParametersDto();

        await testingServiceScope.InsertAsync(worldBuildingAttemptOne, worldBuildingAttemptTwo);

        // Act
        var query = new GetWorldBuildingAttemptList.Query(queryParameters);
        var worldBuildingAttempts = await testingServiceScope.SendAsync(query);

        // Assert
        worldBuildingAttempts.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}