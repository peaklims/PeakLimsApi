namespace PeakLims.FunctionalTests.FunctionalTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class GetContainerTests : TestBase
{
    [Fact]
    public async Task get_container_returns_success_when_entity_exists_using_valid_auth_credentials()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeContainer);

        // Act
        var route = ApiRoutes.Containers.GetRecord(fakeContainer.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
            
    [Fact]
    public async Task get_container_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();

        // Act
        var route = ApiRoutes.Containers.GetRecord(fakeContainer.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task get_container_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Containers.GetRecord(fakeContainer.Id);
        var result = await FactoryClient.GetRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}