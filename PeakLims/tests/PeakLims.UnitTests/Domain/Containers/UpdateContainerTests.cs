namespace PeakLims.UnitTests.Domain.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateContainerTests
{
    private readonly Faker _faker;

    public UpdateContainerTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_container()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        var updatedContainer = new FakeContainerForUpdate().Generate();
        
        // Act
        fakeContainer.Update(updatedContainer);

        // Assert
        fakeContainer.UsedFor.Value.Should().Be(updatedContainer.UsedFor);
        fakeContainer.Type.Should().Be(updatedContainer.Type);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        var updatedContainer = new FakeContainerForUpdate().Generate();
        fakeContainer.DomainEvents.Clear();
        
        // Act
        fakeContainer.Update(updatedContainer);

        // Assert
        fakeContainer.DomainEvents.Count.Should().Be(1);
        fakeContainer.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(ContainerUpdated));
    }
}