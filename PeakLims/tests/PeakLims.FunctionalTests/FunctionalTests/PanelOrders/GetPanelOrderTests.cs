namespace PeakLims.FunctionalTests.FunctionalTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;

public class GetPanelOrderTests : TestBase
{
    [Fact]
    public async Task get_panelorder_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(panelOrder);

        // Act
        var route = ApiRoutes.PanelOrders.GetRecord(panelOrder.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_panelorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();

        // Act
        var route = ApiRoutes.PanelOrders.GetRecord(panelOrder.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_panelorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.PanelOrders.GetRecord(panelOrder.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}