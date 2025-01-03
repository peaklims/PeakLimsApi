namespace PeakLims.UnitTests.Domain.TestOrders;

using Bogus;
using FluentAssertions;
using NSubstitute;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrderStatuses;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Sample;
using Xunit;
using ValidationException = Exceptions.ValidationException;

public class SetStatusToReadyForTestingTests
{
    private readonly Faker _faker;

    public SetStatusToReadyForTestingTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_set_to_ready_for_testing()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var fakeTestOrder = TestOrder.Create(test);
        fakeTestOrder.SetSample(sample);
        
        // Act
        fakeTestOrder.MarkAsReadyForTesting();

        // Assert
        fakeTestOrder.Status.Should().Be(TestOrderStatus.ReadyForTesting());
        
        var dueDate = (DateOnly)fakeTestOrder.DueDate!;
        dueDate.Should().Be(sample.ReceivedDate.AddDays(test.TurnAroundTime));
    }
    
    [Fact]
    public void can_not_set_to_ready_for_testing_when_processing()
    {
        // Arrange
        var container = new FakeContainerBuilder().Build();
        var sample = new FakeSampleBuilder().WithValidContainer(container).Build();
        var test = new FakeTestBuilder().Build().Activate();
        var fakeTestOrder = TestOrder.Create(test);
        fakeTestOrder.SetSample(sample);
        fakeTestOrder.MarkAsReadyForTesting();
        
        // Act
        var actAdd = () => fakeTestOrder.MarkAsReadyForTesting();

        // Assert
        actAdd.Should().Throw<ValidationException>()
            .WithMessage($"Test orders in a {TestOrderStatus.ReadyForTesting().Value} state can not be set to {TestOrderStatus.ReadyForTesting().Value}.");
    }

    [Fact]
    public void must_have_a_sample()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var fakeTestOrder = TestOrder.Create(test);
        
        // Act
        var actAdd = () => fakeTestOrder.MarkAsReadyForTesting();

        // Assert
        actAdd.Should().Throw<ValidationException>()
            .WithMessage($"A sample is required in order to set a test order to {TestOrderStatus.ReadyForTesting().Value}.");
    }
}