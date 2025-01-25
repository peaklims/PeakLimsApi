namespace PeakLims.IntegrationTests.FeatureTests.WorldBuildingPhases;

using PeakLims.Domain.WorldBuildingPhases.Dtos;
using PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;
using PeakLims.Domain.WorldBuildingPhases.Features;
using Domain;
using System.Threading.Tasks;

public class WorldBuildingPhaseListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_worldbuildingphase_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var worldBuildingPhaseOne = new FakeWorldBuildingPhaseBuilder().Build();
        var worldBuildingPhaseTwo = new FakeWorldBuildingPhaseBuilder().Build();
        var queryParameters = new WorldBuildingPhaseParametersDto();

        await testingServiceScope.InsertAsync(worldBuildingPhaseOne, worldBuildingPhaseTwo);

        // Act
        var query = new GetWorldBuildingPhaseList.Query(queryParameters);
        var worldBuildingPhases = await testingServiceScope.SendAsync(query);

        // Assert
        worldBuildingPhases.Count.Should().BeGreaterThanOrEqualTo(2);
    }
}