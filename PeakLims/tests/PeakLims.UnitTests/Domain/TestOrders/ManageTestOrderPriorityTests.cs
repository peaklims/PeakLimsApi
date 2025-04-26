namespace PeakLims.UnitTests.Domain.TestOrders;

using Bogus;
using FluentAssertions;
using NSubstitute;
using PeakLims.Domain.TestOrderPriorities;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrderStatuses;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Sample;
using Xunit;
using ValidationException = Exceptions.ValidationException;

public class ManageTestOrderPriorityTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void can_mark_stat()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        testOrder.SetSample(sample);
        
        // Act
        testOrder.MarkAsStat();

        // Assert
        testOrder.Priority.Should().Be(TestOrderPriority.Stat());
        
        var dueDate = (DateOnly)testOrder.DueDate!;
        dueDate.Should().Be(sample.ReceivedDate.AddDays(test.StatTurnAroundTime));
    }
    [Fact]
    public void can_mark_normal()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        testOrder.SetSample(sample);
        
        // Act
        testOrder.MarkAsStat();
        testOrder.Priority.Should().Be(TestOrderPriority.Stat());
        testOrder.MarkAsNormal();

        // Assert
        testOrder.Priority.Should().Be(TestOrderPriority.Normal());
        
        var dueDate = (DateOnly)testOrder.DueDate!;
        dueDate.Should().Be(sample.ReceivedDate.AddDays(test.TurnAroundTime));
    }

    [Fact]
    public void no_due_date_when_no_sample_for_normal()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        
        // Act
        testOrder.MarkAsNormal();

        // Assert
        testOrder.DueDate.Should().BeNull();
    }
    
    [Fact]
    public void no_due_date_when_no_sample_for_stat()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var testOrder = TestOrder.Create(test);
        
        // Act
        testOrder.MarkAsStat();

        // Assert
        testOrder.DueDate.Should().BeNull();
    }
}