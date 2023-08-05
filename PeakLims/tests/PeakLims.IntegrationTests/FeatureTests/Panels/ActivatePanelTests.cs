namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using System.Threading.Tasks;
using Domain.TestOrders.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain.Panels.Features;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.PanelStatuses;
using PeakLims.SharedTestHelpers.Fakes.Panel;
using Xunit;
using static TestFixture;

public class ActivatePanelTests : TestBase
{
    [Fact]
    public async Task can_activate_panel()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanel = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(fakePanel);

        // Act
        var command = new ActivatePanel.Command(fakePanel.Id);
        await testingServiceScope.SendAsync(command);
        var updatedPanel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels.FirstOrDefaultAsync(a => a.Id == fakePanel.Id));

        // Assert
        updatedPanel.Status.Should().Be(PanelStatus.Active());
    }
}