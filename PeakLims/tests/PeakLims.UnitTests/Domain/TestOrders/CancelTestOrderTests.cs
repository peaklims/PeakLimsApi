namespace PeakLims.UnitTests.Domain.TestOrders;

using Bogus;
using FluentAssertions;
using PeakLims.Domain.TestOrderCancellationReasons;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrderStatuses;
using PeakLims.SharedTestHelpers.Fakes.Test;
using Xunit;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class CancelTestOrderTests
{
    private readonly Faker _faker;

    public CancelTestOrderTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_cancel_test_order()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var reason = _faker.PickRandom(TestOrderCancellationReason.ListNames());
        var comments = _faker.Lorem.Sentence(); 
        
        // Act
        var fakeTestOrder = TestOrder.Create(test);
        fakeTestOrder.Cancel(TestOrderCancellationReason.Of(reason), comments);

        // Assert
        fakeTestOrder.Status.Should().Be(TestOrderStatus.Cancelled());
        fakeTestOrder.CancellationReason.Value.Should().Be(reason);
        fakeTestOrder.CancellationComments.Should().Be(comments);
    }
    
    [Fact]
    public void must_have_comment_when_cancelling()
    {
        // Arrange
        var test = new FakeTestBuilder().Build().Activate();
        var reason = _faker.PickRandom(TestOrderCancellationReason.ListNames());
        
        // Act
        var fakeTestOrder = TestOrder.Create(test);
        var act = () => fakeTestOrder.Cancel(TestOrderCancellationReason.Of(reason), null);

        // Assert
        act.Should().Throw<ValidationException>()
            .WithMessage("A comment must be provided detailing why the test order was cancelled.");
    }
}