namespace PeakLims.FunctionalTests.FunctionalTests.Panels;

using PeakLims.SharedTestHelpers.Fakes.Panel;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetPanelTests : TestBase
{
    [Fact]
    public async Task get_panel_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakePanel);

        // Act
        var route = ApiRoutes.Panels.GetRecord(fakePanel.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_panel_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();

        // Act
        var route = ApiRoutes.Panels.GetRecord(fakePanel.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_panel_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakePanel = new FakePanelBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Panels.GetRecord(fakePanel.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}