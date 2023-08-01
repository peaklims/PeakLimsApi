namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.TestOrders.Features;
using SharedKernel.Exceptions;

public class AddTestOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_testorder_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderForCreationDto().Generate();

        // Act
        var command = new AddTestOrder.Command(fakeTestOrderOne);
        var testOrderReturned = await testingServiceScope.SendAsync(command);
        var testOrderCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(t => t.Id == testOrderReturned.Id));

        // Assert
        testOrderReturned.Status.Should().Be(fakeTestOrderOne.Status);
        testOrderReturned.DueDate.Should().Be(fakeTestOrderOne.DueDate);
        testOrderReturned.TatSnapshot.Should().Be(fakeTestOrderOne.TatSnapshot);
        testOrderReturned.CancellationReason.Should().Be(fakeTestOrderOne.CancellationReason);
        testOrderReturned.CancellationComments.Should().Be(fakeTestOrderOne.CancellationComments);
        testOrderReturned.AssociatedPanelId.Should().Be(fakeTestOrderOne.AssociatedPanelId);

        testOrderCreated.Status.Should().Be(fakeTestOrderOne.Status);
        testOrderCreated.DueDate.Should().Be(fakeTestOrderOne.DueDate);
        testOrderCreated.TatSnapshot.Should().Be(fakeTestOrderOne.TatSnapshot);
        testOrderCreated.CancellationReason.Should().Be(fakeTestOrderOne.CancellationReason);
        testOrderCreated.CancellationComments.Should().Be(fakeTestOrderOne.CancellationComments);
        testOrderCreated.AssociatedPanelId.Should().Be(fakeTestOrderOne.AssociatedPanelId);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddTestOrders);
        var fakeTestOrderOne = new FakeTestOrderForCreationDto();

        // Act
        var command = new AddTestOrder.Command(fakeTestOrderOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}