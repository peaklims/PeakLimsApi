namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders.Features;
using Microsoft.EntityFrameworkCore;
using Domain;
using System.Threading.Tasks;

public class DeletePanelOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_panelorder_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var panelOrderOne = new FakePanelOrderBuilder().Build();
        await testingServiceScope.InsertAsync(panelOrderOne);
        var panelOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .FirstOrDefaultAsync(p => p.Id == panelOrderOne.Id));

        // Act
        var command = new DeletePanelOrder.Command(panelOrder.Id);
        await testingServiceScope.SendAsync(command);
        var panelOrderResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders.CountAsync(p => p.Id == panelOrder.Id));

        // Assert
        panelOrderResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_panelorder_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeletePanelOrder.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_panelorder_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var panelOrderOne = new FakePanelOrderBuilder().Build();
        await testingServiceScope.InsertAsync(panelOrderOne);
        var panelOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .FirstOrDefaultAsync(p => p.Id == panelOrderOne.Id));

        // Act
        var command = new DeletePanelOrder.Command(panelOrder.Id);
        await testingServiceScope.SendAsync(command);
        var deletedPanelOrder = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == panelOrder.Id));

        // Assert
        deletedPanelOrder?.IsDeleted.Should().BeTrue();
    }

    [Fact(Skip = "need to redo permission granularity")]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeletePanelOrders);

        // Act
        var command = new DeletePanelOrder.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}