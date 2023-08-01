namespace PeakLims.UnitTests.Domain.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

public class UpdateTestOrderTests
{
    private readonly Faker _faker;

    public UpdateTestOrderTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_update_testOrder()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        var updatedTestOrder = new FakeTestOrderForUpdate().Generate();
        
        // Act
        fakeTestOrder.Update(updatedTestOrder);

        // Assert
        fakeTestOrder.Status.Should().Be(updatedTestOrder.Status);
        fakeTestOrder.DueDate.Should().Be(updatedTestOrder.DueDate);
        fakeTestOrder.TatSnapshot.Should().Be(updatedTestOrder.TatSnapshot);
        fakeTestOrder.CancellationReason.Should().Be(updatedTestOrder.CancellationReason);
        fakeTestOrder.CancellationComments.Should().Be(updatedTestOrder.CancellationComments);
        fakeTestOrder.AssociatedPanelId.Should().Be(updatedTestOrder.AssociatedPanelId);
    }
    
    [Fact]
    public void queue_domain_event_on_update()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        var updatedTestOrder = new FakeTestOrderForUpdate().Generate();
        fakeTestOrder.DomainEvents.Clear();
        
        // Act
        fakeTestOrder.Update(updatedTestOrder);

        // Assert
        fakeTestOrder.DomainEvents.Count.Should().Be(1);
        fakeTestOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestOrderUpdated));
    }
}