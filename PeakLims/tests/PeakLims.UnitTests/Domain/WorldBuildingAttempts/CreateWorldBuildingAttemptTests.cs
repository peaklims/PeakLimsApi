namespace PeakLims.UnitTests.Domain.WorldBuildingAttempts;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingAttempts.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class CreateWorldBuildingAttemptTests
{
    private readonly Faker _faker;

    public CreateWorldBuildingAttemptTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_worldBuildingAttempt()
    {
        // Arrange
        var worldBuildingAttemptToCreate = new FakeWorldBuildingAttemptForCreation().Generate();
        
        // Act
        var worldBuildingAttempt = WorldBuildingAttempt.CreateStandardWorld(worldBuildingAttemptToCreate);

        // Assert
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var worldBuildingAttemptToCreate = new FakeWorldBuildingAttemptForCreation().Generate();
        
        // Act
        var worldBuildingAttempt = WorldBuildingAttempt.CreateStandardWorld(worldBuildingAttemptToCreate);

        // Assert
        worldBuildingAttempt.DomainEvents.Count.Should().Be(1);
        worldBuildingAttempt.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(WorldBuildingAttemptCreated));
    }
}