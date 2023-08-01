namespace PeakLims.FunctionalTests.FunctionalTests.Containers;

using PeakLims.SharedTestHelpers.Fakes.Container;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateContainerRecordTests : TestBase
{
    [Fact]
    public async Task put_container_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        var updatedContainerDto = new FakeContainerForUpdateDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeContainer);

        // Act
        var route = ApiRoutes.Containers.Put(fakeContainer.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedContainerDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task put_container_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        var updatedContainerDto = new FakeContainerForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.Containers.Put(fakeContainer.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedContainerDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_container_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeContainer = new FakeContainerBuilder().Build();
        var updatedContainerDto = new FakeContainerForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Containers.Put(fakeContainer.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedContainerDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}