namespace PeakLims.FunctionalTests.FunctionalTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateTestOrderRecordTests : TestBase
{
    [Fact]
    public async Task put_testorder_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        var updatedTestOrderDto = new FakeTestOrderForUpdateDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeTestOrder);

        // Act
        var route = ApiRoutes.TestOrders.Put(fakeTestOrder.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestOrderDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task put_testorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        var updatedTestOrderDto = new FakeTestOrderForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.TestOrders.Put(fakeTestOrder.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestOrderDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_testorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderBuilder().Build();
        var updatedTestOrderDto = new FakeTestOrderForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.TestOrders.Put(fakeTestOrder.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestOrderDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}