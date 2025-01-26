namespace PeakLims.UnitTests.Domain.WorldBuildingPhases;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Domain.WorldBuildingPhases.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdateWorldBuildingPhaseTests
{
    private readonly Faker _faker;

    public UpdateWorldBuildingPhaseTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_worldBuildingPhase()
    {
        // Arrange
        var worldBuildingPhase = new FakeWorldBuildingPhaseBuilder().Build();
        var updatedWorldBuildingPhase = new FakeWorldBuildingPhaseForUpdate().Generate();
        
        // Act
        worldBuildingPhase.Update(updatedWorldBuildingPhase);

        // Assert
        worldBuildingPhase.ResultData.Should().Be(updatedWorldBuildingPhase.ResultData);
        worldBuildingPhase.StartedAt.Should().BeCloseTo((DateTimeOffset)updatedWorldBuildingPhase.StartedAt, 1.Seconds());
        worldBuildingPhase.EndedAt.Should().BeCloseTo((DateTimeOffset)updatedWorldBuildingPhase.EndedAt, 1.Seconds());
        worldBuildingPhase.StepNumber.Should().Be(updatedWorldBuildingPhase.StepNumber);
        worldBuildingPhase.Comments.Should().Be(updatedWorldBuildingPhase.Comments);
        worldBuildingPhase.ErrorMessage.Should().Be(updatedWorldBuildingPhase.ErrorMessage);
        worldBuildingPhase.SpecialRequests.Should().Be(updatedWorldBuildingPhase.SpecialRequests);
        worldBuildingPhase.AttemptNumber.Should().Be(updatedWorldBuildingPhase.AttemptNumber);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var worldBuildingPhase = new FakeWorldBuildingPhaseBuilder().Build();
        var updatedWorldBuildingPhase = new FakeWorldBuildingPhaseForUpdate().Generate();
        worldBuildingPhase.DomainEvents.Clear();
        
        // Act
        worldBuildingPhase.Update(updatedWorldBuildingPhase);

        // Assert
        worldBuildingPhase.DomainEvents.Count.Should().Be(1);
        worldBuildingPhase.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(WorldBuildingPhaseUpdated));
    }
}