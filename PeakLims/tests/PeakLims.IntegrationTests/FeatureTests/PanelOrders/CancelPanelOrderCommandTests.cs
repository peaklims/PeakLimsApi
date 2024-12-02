namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.Domain.PanelOrders.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.TestOrderCancellationReasons;
using Domain.PanelOrderStatuses;
using static TestFixture;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Container;
using SharedTestHelpers.Fakes.Patient;
using SharedTestHelpers.Fakes.Sample;
using SharedTestHelpers.Fakes.Test;

public class CancelPanelOrderCommandTests : TestBase
{
    [Fact]
    public async Task can_cancel_panel_order()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        
        var test = new FakeTestBuilder().Build().Activate();
        var panel = new FakePanelBuilder().Build()
            .AddTest(test)
            .Activate();
        
        var accession = new FakeAccessionBuilder().Build();
        var panelOrder = accession.AddPanel(panel);
        await testingServiceScope.InsertAsync(accession);
        
        var reason = TestOrderCancellationReason.Other();
        var comments = "Test cancellation";
        
        // Act
        var command = new CancelPanelOrder.Command(panelOrder.Id, reason, comments);
        await testingServiceScope.SendAsync(command);
        
        // Assert
        var panelOrderFromDb = await testingServiceScope.ExecuteDbContextAsync(db => db.PanelOrders
            .Include(x => x.TestOrders)
            .FirstOrDefaultAsync(x => x.Id == panelOrder.Id));
        
        panelOrderFromDb.Status().Should().Be(PanelOrderStatus.Cancelled());
        panelOrderFromDb.CancellationReason.Should().Be(reason);
        panelOrderFromDb.CancellationComments.Should().Be(comments);
        
        foreach (var testOrder in panelOrderFromDb.TestOrders)
        {
            testOrder.Status.Should().Be(Domain.TestOrderStatuses.TestOrderStatus.Cancelled());
            testOrder.CancellationReason.Should().Be(reason);
            testOrder.CancellationComments.Should().Be(comments);
        }
    }
}
