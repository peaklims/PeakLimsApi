namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PeakLims.Domain.PanelOrders.Features;

public class AddPanelOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_panelorder_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var panelOrderOne = new FakePanelOrderForCreationDto().Generate();

        // Act
        var command = new AddPanelOrder.Command(panelOrderOne);
        var panelOrderReturned = await testingServiceScope.SendAsync(command);
        var panelOrderCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .FirstOrDefaultAsync(p => p.Id == panelOrderReturned.Id));

        // Assert
        panelOrderReturned.CancellationReason.Should().Be(panelOrderOne.CancellationReason);
        panelOrderReturned.CancellationComments.Should().Be(panelOrderOne.CancellationComments);

        panelOrderCreated.CancellationReason.Should().Be(panelOrderOne.CancellationReason);
        panelOrderCreated.CancellationComments.Should().Be(panelOrderOne.CancellationComments);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddPanelOrders);
        var panelOrderOne = new FakePanelOrderForCreationDto();

        // Act
        var command = new AddPanelOrder.Command(panelOrderOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}