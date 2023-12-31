namespace PeakLims.IntegrationTests.FeatureTests.PanelOrders;

using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.Domain.PanelOrders.Features;
using Domain;
using System.Threading.Tasks;

public class PanelOrderListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_panelorder_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var panelOrderOne = new FakePanelOrderBuilder().Build();
        var panelOrderTwo = new FakePanelOrderBuilder().Build();
        var queryParameters = new PanelOrderParametersDto();

        await testingServiceScope.InsertAsync(panelOrderOne, panelOrderTwo);

        // Act
        var query = new GetPanelOrderList.Query(queryParameters);
        var panelOrders = await testingServiceScope.SendAsync(query);

        // Assert
        panelOrders.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadPanelOrders);
        var queryParameters = new PanelOrderParametersDto();

        // Act
        var command = new GetPanelOrderList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}