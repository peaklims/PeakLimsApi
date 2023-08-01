namespace PeakLims.UnitTests.Domain.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class CreateTestOrderTests
{
    private readonly Faker _faker;

    public CreateTestOrderTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_testOrder()
    {
        // Arrange
        var testOrderToCreate = new FakeTestOrderForCreation().Generate();
        
        // Act
        var fakeTestOrder = TestOrder.Create(testOrderToCreate);

        // Assert
        fakeTestOrder.Status.Should().Be(testOrderToCreate.Status);
        fakeTestOrder.DueDate.Should().Be(testOrderToCreate.DueDate);
        fakeTestOrder.TatSnapshot.Should().Be(testOrderToCreate.TatSnapshot);
        fakeTestOrder.CancellationReason.Should().Be(testOrderToCreate.CancellationReason);
        fakeTestOrder.CancellationComments.Should().Be(testOrderToCreate.CancellationComments);
        fakeTestOrder.AssociatedPanelId.Should().Be(testOrderToCreate.AssociatedPanelId);
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var testOrderToCreate = new FakeTestOrderForCreation().Generate();
        
        // Act
        var fakeTestOrder = TestOrder.Create(testOrderToCreate);

        // Assert
        fakeTestOrder.DomainEvents.Count.Should().Be(1);
        fakeTestOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestOrderCreated));
    }
}