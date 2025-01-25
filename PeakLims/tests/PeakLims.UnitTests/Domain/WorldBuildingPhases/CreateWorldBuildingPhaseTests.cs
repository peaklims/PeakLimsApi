namespace PeakLims.UnitTests.Domain.WorldBuildingPhases;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingPhase;
using PeakLims.Domain.WorldBuildingPhases;
using PeakLims.Domain.WorldBuildingPhases.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreateWorldBuildingPhaseTests
{
    private readonly Faker _faker;

    public CreateWorldBuildingPhaseTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_worldBuildingPhase()
    {
        // Arrange
        var worldBuildingPhaseToCreate = new FakeWorldBuildingPhaseForCreation().Generate();
        
        // Act
        var worldBuildingPhase = WorldBuildingPhase.Create(worldBuildingPhaseToCreate);

        // Assert
        worldBuildingPhase.ResultData.Should().Be(worldBuildingPhaseToCreate.ResultData);
        worldBuildingPhase.StartedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseToCreate.StartedAt, 1.Seconds());
        worldBuildingPhase.EndedAt.Should().BeCloseTo((DateTimeOffset)worldBuildingPhaseToCreate.EndedAt, 1.Seconds());
        worldBuildingPhase.StepNumber.Should().Be(worldBuildingPhaseToCreate.StepNumber);
        worldBuildingPhase.Comments.Should().Be(worldBuildingPhaseToCreate.Comments);
        worldBuildingPhase.ErrorMessage.Should().Be(worldBuildingPhaseToCreate.ErrorMessage);
        worldBuildingPhase.SpecialRequests.Should().Be(worldBuildingPhaseToCreate.SpecialRequests);
        worldBuildingPhase.AttemptNumber.Should().Be(worldBuildingPhaseToCreate.AttemptNumber);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var worldBuildingPhaseToCreate = new FakeWorldBuildingPhaseForCreation().Generate();
        
        // Act
        var worldBuildingPhase = WorldBuildingPhase.Create(worldBuildingPhaseToCreate);

        // Assert
        worldBuildingPhase.DomainEvents.Count.Should().Be(1);
        worldBuildingPhase.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(WorldBuildingPhaseCreated));
    }
}