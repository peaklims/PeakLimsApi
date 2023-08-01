namespace PeakLims.IntegrationTests.FeatureTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.Domain.Panels.Features;
using SharedKernel.Exceptions;
using Domain;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class PanelQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_panel_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakePanelOne = new FakePanelBuilder().Build();
        await testingServiceScope.InsertAsync(fakePanelOne);

        // Act
        var query = new GetPanel.Query(fakePanelOne.Id);
        var panel = await testingServiceScope.SendAsync(query);

        // Assert
        panel.PanelCode.Should().Be(fakePanelOne.PanelCode);
        panel.PanelName.Should().Be(fakePanelOne.PanelName);
        panel.Type.Should().Be(fakePanelOne.Type);
        panel.Version.Should().Be(fakePanelOne.Version);
        panel.Status.Should().Be(fakePanelOne.Status);
    }

    [Fact]
    public async Task get_panel_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetPanel.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPanels);

        // Act
        var command = new GetPanel.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}