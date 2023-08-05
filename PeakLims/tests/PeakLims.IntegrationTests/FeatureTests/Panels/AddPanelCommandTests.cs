namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.Panels.Features;
using SharedKernel.Exceptions;

public class AddPanelCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_panel_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelForCreationDto().Generate();

        // Act
        var command = new AddPanel.Command(fakePanelOne);
        var panelReturned = await testingServiceScope.SendAsync(command);
        var panelCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .FirstOrDefaultAsync(p => p.Id == panelReturned.Id));

        // Assert
        panelReturned.PanelCode.Should().Be(fakePanelOne.PanelCode);
        panelReturned.PanelName.Should().Be(fakePanelOne.PanelName);
        panelReturned.Type.Should().Be(fakePanelOne.Type);
        
        panelCreated.PanelCode.Should().Be(fakePanelOne.PanelCode);
        panelCreated.PanelName.Should().Be(fakePanelOne.PanelName);
        panelCreated.Type.Should().Be(fakePanelOne.Type);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanAddPanels);
        var fakePanelOne = new FakePanelForCreationDto();

        // Act
        var command = new AddPanel.Command(fakePanelOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}