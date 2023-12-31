namespace PeakLims.FunctionalTests.FunctionalTests.PanelOrders;

using PeakLims.SharedTestHelpers.Fakes.PanelOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using System.Net;
using System.Threading.Tasks;

public class CreatePanelOrderTests : TestBase
{
    [Fact]
    public async Task create_panelorder_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var panelOrder = new FakePanelOrderForCreationDto().Generate();

        var callingUser = await AddNewSuperAdmin();
        FactoryClient.AddAuth(callingUser.Identifier);

        // Act
        var route = ApiRoutes.PanelOrders.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, panelOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_panelorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var panelOrder = new FakePanelOrderForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.PanelOrders.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, panelOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_panelorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var panelOrder = new FakePanelOrderForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.PanelOrders.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, panelOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}