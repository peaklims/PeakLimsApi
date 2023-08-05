namespace PeakLims.UnitTests.Domain.TestOrders;

using Bogus;
using FluentAssertions;
using Moq;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrderStatuses;
using PeakLims.Services;
using PeakLims.SharedTestHelpers.Fakes.Test;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Sample;
using Xunit;

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
        fakeTestOrder.SetStatusToReadyForTesting();

        // Assert
        fakeTestOrder.Status.Should().Be(TestOrderStatus.ReadyForTesting());
        fakeTestOrder.TatSnapshot.Should().Be(test.TurnAroundTime);
        
        var dueDate = (DateOnly)fakeTestOrder.DueDate!;
        dueDate.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(test.TurnAroundTime)));
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
        fakeTestOrder.SetStatusToReadyForTesting();
        
        // Act
        var actAdd = () => fakeTestOrder.SetStatusToReadyForTesting();

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"Test orders in a {TestOrderStatus.ReadyForTesting().Value} state can not be set to {TestOrderStatus.ReadyForTesting().Value}.");
    }

    [Fact]
    public void must_have_a_sample()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var fakeTestOrder = TestOrder.Create(test);
        
        // Act
        var actAdd = () => fakeTestOrder.SetStatusToReadyForTesting();

        // Assert
        actAdd.Should().Throw<SharedKernel.Exceptions.ValidationException>()
            .WithMessage($"A sample is required in order to set a test order to {TestOrderStatus.ReadyForTesting().Value}.");
    }
}