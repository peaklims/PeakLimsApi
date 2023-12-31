namespace PeakLims.FunctionalTests.FunctionalTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;

public class DeletePanelOrderTests : TestBase
{
    [Fact]
    public async Task delete_panelorder_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);
        await InsertAsync(panelOrder);

        // Act
        var route = ApiRoutes.PanelOrders.Delete(panelOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_panelorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();

        // Act
        var route = ApiRoutes.PanelOrders.Delete(panelOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_panelorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var panelOrder = new FakePanelOrderBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.PanelOrders.Delete(panelOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}