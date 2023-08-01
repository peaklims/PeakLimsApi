namespace PeakLims.UnitTests.Domain.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.Domain.Containers;
using PeakLims.Domain.Containers.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateContainerTests
{
    private readonly Faker _faker;

    public CreateContainerTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_container()
    {
        // Arrange
        var containerToCreate = new FakeContainerForCreation().Generate();
        
        // Act
        var fakeContainer = Container.Create(containerToCreate);

        // Assert
        fakeContainer.UsedFor.Should().Be(containerToCreate.UsedFor);
        fakeContainer.Status.Should().Be(containerToCreate.Status);
        fakeContainer.Type.Should().Be(containerToCreate.Type);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var containerToCreate = new FakeContainerForCreation().Generate();
        
        // Act
        var fakeContainer = Container.Create(containerToCreate);

        // Assert
        fakeContainer.DomainEvents.Count.Should().Be(1);
        fakeContainer.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(ContainerCreated));
    }
}