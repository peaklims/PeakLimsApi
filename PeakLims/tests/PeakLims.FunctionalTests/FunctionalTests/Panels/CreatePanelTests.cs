namespace PeakLims.FunctionalTests.FunctionalTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreatePanelTests : TestBase
{
    [Fact]
    public async Task create_panel_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakePanel = new FakePanelForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.Panels.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakePanel);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_panel_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakePanel = new FakePanelForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.Panels.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakePanel);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_panel_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakePanel = new FakePanelForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Panels.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakePanel);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}