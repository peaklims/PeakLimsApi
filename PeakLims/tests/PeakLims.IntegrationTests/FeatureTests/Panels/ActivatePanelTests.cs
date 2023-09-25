namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using System.Threading.Tasks;
using Domain.TestOrders.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Panels.Features;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.PanelStatuses;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;
using static TestFixture;

public class ActivatePanelTests : TestBase
{
    [Fact]
    public async Task can_activate_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var test = new FakeTestBuilder().Build().Activate();
        await testingServiceScope.InsertAsync(test);
        
        var panel = new FakePanelBuilder().Build();
        panel = panel.AddTest(test);
        await testingServiceScope.InsertAsync(panel);

        // Act
        var command = new ActivatePanel.Command(panel.Id);
        await testingServiceScope.SendAsync(command);
        var updatedPanel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels.FirstOrDefaultAsync(a => a.Id == panel.Id));

        // Assert
        updatedPanel.Status.Should().Be(PanelStatus.Active());
    }
}