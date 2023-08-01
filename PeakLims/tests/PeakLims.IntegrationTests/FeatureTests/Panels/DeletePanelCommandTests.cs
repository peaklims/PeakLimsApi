namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Domain;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class DeletePanelCommandTests : TestBase
{
    [Fact]
    public async Task can_delete_panel_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(fakePanelOne);
        var panel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .FirstOrDefaultAsync(p => p.Id == fakePanelOne.Id));

        // Act
        var command = new DeletePanel.Command(panel.Id);
        await testingServiceScope.SendAsync(command);
        var panelResponse = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels.CountAsync(p => p.Id == panel.Id));

        // Assert
        panelResponse.Should().Be(0);
    }

    [Fact]
    public async Task delete_panel_throws_notfoundexception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var command = new DeletePanel.Command(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task can_softdelete_panel_from_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(fakePanelOne);
        var panel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .FirstOrDefaultAsync(p => p.Id == fakePanelOne.Id));

        // Act
        var command = new DeletePanel.Command(panel.Id);
        await testingServiceScope.SendAsync(command);
        var deletedPanel = await testingServiceScope.ExecuteDbContextAsync(db => db.Panels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == panel.Id));

        // Assert
        deletedPanel?.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanDeletePanels);

        // Act
        var command = new DeletePanel.Command(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}