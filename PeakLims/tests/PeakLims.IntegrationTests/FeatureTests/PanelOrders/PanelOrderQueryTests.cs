namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.Domain.PanelOrders.Features;
using Domain;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Panel;
using SharedTestHelpers.Fakes.Test;

public class PanelOrderQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_panelorder_with_accurate_props()
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

        // Act
        var query = new GetPanelOrder.Query(panelOrder.Id);
        var dbPanelOrder = await testingServiceScope.SendAsync(query);

        // Assert
        dbPanelOrder.CancellationReason.Should().Be(panelOrder.CancellationReason);
        dbPanelOrder.CancellationComments.Should().Be(panelOrder.CancellationComments);
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

    [Fact(Skip = "need to redo permission granularity")]
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