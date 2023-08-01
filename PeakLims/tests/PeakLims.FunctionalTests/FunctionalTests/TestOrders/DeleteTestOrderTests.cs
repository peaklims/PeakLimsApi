namespace PeakLims.FunctionalTests.FunctionalTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteTestOrderTests : TestBase
{
    [Fact]
    public async Task delete_testorder_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeTestOrder);

        // Act
        var route = ApiRoutes.TestOrders.Delete(fakeTestOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_testorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();

        // Act
        var route = ApiRoutes.TestOrders.Delete(fakeTestOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_testorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.TestOrders.Delete(fakeTestOrder.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}