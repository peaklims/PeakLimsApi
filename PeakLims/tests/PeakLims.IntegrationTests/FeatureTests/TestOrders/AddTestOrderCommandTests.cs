namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Domain.TestOrderStatuses;
using PeakLims.Domain.TestOrders.Features;
using SharedKernel.Exceptions;
using SharedTestHelpers.Fakes.Test;

public class AddTestOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_testorder_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTest = new FakeTestBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTest);

        // Act
        var command = new AddTestOrder.Command(fakeTest.Id, null);
        var testOrderReturned = await testingServiceScope.SendAsync(command);
        var testOrderCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .Include(t => t.Test)
            .FirstOrDefaultAsync(t => t.Id == testOrderReturned.Id));

        // Assert
        testOrderReturned.Status.Should().Be(TestOrderStatus.Pending());
        testOrderReturned.TestId.Should().Be(fakeTest.Id);

        testOrderCreated.Status.Should().Be(TestOrderStatus.Pending());
        testOrderCreated.Test.Id.Should().Be(fakeTest.Id);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddTestOrders);
        
        // Act
        var command = new AddTestOrder.Command(Guid.NewGuid(), null);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}