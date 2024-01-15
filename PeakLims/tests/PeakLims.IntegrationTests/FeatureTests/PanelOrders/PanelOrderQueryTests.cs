namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class PanelOrderQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_panelorder_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var panelOrderOne = new FakePanelOrderBuilder().Build();
        await testingServiceScope.InsertAsync(panelOrderOne);

        // Act
        var query = new GetPanelOrder.Query(panelOrderOne.Id);
        var panelOrder = await testingServiceScope.SendAsync(query);

        // Assert
        panelOrder.CancellationReason.Should().Be(panelOrderOne.CancellationReason);
        panelOrder.CancellationComments.Should().Be(panelOrderOne.CancellationComments);
    }

    [Fact]
    public async Task get_panelorder_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetPanelOrder.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPanelOrders);

        // Act
        var command = new GetPanelOrder.Query(Guid.NewGuid());
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}