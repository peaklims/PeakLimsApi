namespace PeakLims.IntegrationTests.FeatureTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.Domain.TestOrders.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.TestOrders.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateTestOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_testorder_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeTestOrderOne = new FakeTestOrderBuilder().Build();
        var updatedTestOrderDto = new FakeTestOrderForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeTestOrderOne);

        var testOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders
            .FirstOrDefaultAsync(t => t.Id == fakeTestOrderOne.Id));

        // Act
        var command = new UpdateTestOrder.Command(testOrder.Id, updatedTestOrderDto);
        await testingServiceScope.SendAsync(command);
        var updatedTestOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.TestOrders.FirstOrDefaultAsync(t => t.Id == testOrder.Id));

        // Assert
        updatedTestOrder.Status.Should().Be(updatedTestOrderDto.Status);
        updatedTestOrder.DueDate.Should().Be(updatedTestOrderDto.DueDate);
        updatedTestOrder.TatSnapshot.Should().Be(updatedTestOrderDto.TatSnapshot);
        updatedTestOrder.CancellationReason.Should().Be(updatedTestOrderDto.CancellationReason);
        updatedTestOrder.CancellationComments.Should().Be(updatedTestOrderDto.CancellationComments);
        updatedTestOrder.AssociatedPanelId.Should().Be(updatedTestOrderDto.AssociatedPanelId);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdateTestOrders);
        var fakeTestOrderOne = new FakeTestOrderForUpdateDto();

        // Act
        var command = new UpdateTestOrder.Command(Guid.NewGuid(), fakeTestOrderOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}