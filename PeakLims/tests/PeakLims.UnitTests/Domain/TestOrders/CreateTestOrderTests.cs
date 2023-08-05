namespace PeakLims.UnitTests.Domain.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.DomainEvents;
using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using PeakLims.Domain.TestOrderStatuses;
using SharedTestHelpers.Fakes.Test;
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
        var fakeTest = new FakeTestBuilder().Build();
        
        // Act
        var fakeTestOrder = TestOrder.Create(fakeTest);

        // Assert
        fakeTestOrder.Status.Should().Be(TestOrderStatus.Pending());
    }

    [Fact]
    public void queue_domain_event_on_create()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        
        // Act
        var fakeTestOrder = TestOrder.Create(fakeTest);

        // Assert
        fakeTestOrder.DomainEvents.Count.Should().Be(1);
        fakeTestOrder.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(TestOrderCreated));
    }
}