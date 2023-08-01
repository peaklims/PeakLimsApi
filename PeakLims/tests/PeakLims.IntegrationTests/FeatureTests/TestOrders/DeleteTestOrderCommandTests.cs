namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeleteTestOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_testorder_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);
        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(t => t.Id == fakeTestOrderOne.Id));

        // Act
        var command = new DeleteTestOrder.Command(testOrder.Id);
        await testingServiceScope.SendAsync(command);
        var testOrderResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders.CountAsync(t => t.Id == testOrder.Id));

        // Assert
        testOrderResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_testorder_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeleteTestOrder.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_testorder_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderBuilder().Build();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);
        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(t => t.Id == fakeTestOrderOne.Id));

        // Act
        var command = new DeleteTestOrder.Command(testOrder.Id);
        await testingServiceScope.SendAsync(command);
        var deletedTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == testOrder.Id));

        // Assert
        deletedTestOrder?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeleteTestOrders);

        // Act
        var command = new DeleteTestOrder.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}