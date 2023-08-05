namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.Panels.Features;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdatePanelCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_panel_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelBuilder().Build();
        var updatedPanelDto = new FakePanelForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakePanelOne);

        var panel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .FirstOrDefaultAsync(p => p.Id == fakePanelOne.Id));

        // Act
        var command = new UpdatePanel.Command(panel.Id, updatedPanelDto);
        await testingServiceScope.SendAsync(command);
        var updatedPanel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels.FirstOrDefaultAsync(p => p.Id == panel.Id));

        // Assert
        updatedPanel.PanelCode.Should().Be(updatedPanelDto.PanelCode);
        updatedPanel.PanelName.Should().Be(updatedPanelDto.PanelName);
        updatedPanel.Type.Should().Be(updatedPanelDto.Type);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanUpdatePanels);
        var fakePanelOne = new FakePanelForUpdateDto();

        // Act
        var command = new UpdatePanel.Command(Guid.NewGuid(), fakePanelOne);
        var act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}