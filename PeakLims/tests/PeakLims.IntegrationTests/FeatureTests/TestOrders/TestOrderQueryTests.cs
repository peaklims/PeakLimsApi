namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class TestOrderQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_testorder_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);

        // Act
        var query = new GetTestOrder.Query(fakeTestOrderOne.Id);
        var testOrder = await testingServiceScope.SendAsync(query);

        // Assert
        testOrder.Status.Should().Be(fakeTestOrderOne.Status);
        testOrder.DueDate.Should().Be(fakeTestOrderOne.DueDate);
        testOrder.TatSnapshot.Should().Be(fakeTestOrderOne.TatSnapshot);
        testOrder.CancellationReason.Should().Be(fakeTestOrderOne.CancellationReason);
        testOrder.CancellationComments.Should().Be(fakeTestOrderOne.CancellationComments);
    }

    [Fact]
    public async Task get_testorder_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetTestOrder.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadTestOrders);

        // Act
        var command = new GetTestOrder.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}