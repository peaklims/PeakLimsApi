namespace PeakLims.FunctionalTests.FunctionalTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeletePanelTests : TestBase
{
    [Fact]
    public async Task delete_panel_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakePanel);

        // Act
        var route = ApiRoutes.Panels.Delete(fakePanel.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_panel_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();

        // Act
        var route = ApiRoutes.Panels.Delete(fakePanel.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_panel_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Panels.Delete(fakePanel.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}