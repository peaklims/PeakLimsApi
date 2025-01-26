namespace PeakLims.UnitTests.Domain.WorldBuildingAttempts;

using PeakLims.SharedTestHelpers.Fakes.WorldBuildingAttempt;
using PeakLims.Domain.WorldBuildingAttempts;
using PeakLims.Domain.WorldBuildingAttempts.DomainEvents;
using Bogus;
using FluentAssertions.Extensions;
using ValidationException = PeakLims.Exceptions.ValidationException;

public class UpdateWorldBuildingAttemptTests
{
    private readonly Faker _faker;

    public UpdateWorldBuildingAttemptTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_worldBuildingAttempt()
    {
        // Arrange
        var worldBuildingAttempt = new FakeWorldBuildingAttemptBuilder().Build();
        var updatedWorldBuildingAttempt = new FakeWorldBuildingAttemptForUpdate().Generate();
        
        // Act
        worldBuildingAttempt.Update(updatedWorldBuildingAttempt);

        // Assert
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var worldBuildingAttempt = new FakeWorldBuildingAttemptBuilder().Build();
        var updatedWorldBuildingAttempt = new FakeWorldBuildingAttemptForUpdate().Generate();
        worldBuildingAttempt.DomainEvents.Clear();
        
        // Act
        worldBuildingAttempt.Update(updatedWorldBuildingAttempt);

        // Assert
        worldBuildingAttempt.DomainEvents.Count.Should().Be(1);
        worldBuildingAttempt.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(WorldBuildingAttemptUpdated));
    }
}