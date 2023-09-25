namespace PeakLims.FunctionalTests.FunctionalTests.TestOrders;

using PeakLims.SharedTestHelpers.Fakes.TestOrder;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;
using Domain.TestOrders.Dtos;
using SharedTestHelpers.Fakes.Accession;
using SharedTestHelpers.Fakes.Test;

public class CreateTestOrderTests : TestBase
{
    [Fact]
    public async Task create_testorder_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var accession = new FakeAccessionBuilder().Build();
        await InsertAsync(accession);
        
        var test = new FakeTestBuilder().Build().Activate();
        await InsertAsync(test);
        
        var fakeTestOrder = new TestOrderForCreationDto();
        fakeTestOrder.TestId = test.Id;
        fakeTestOrder.PanelId = null;

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.TestOrders.Create(accession.Id);
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTestOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task create_testorder_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.TestOrders.Create(Guid.NewGuid());
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTestOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_testorder_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeTestOrder = new FakeTestOrderForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.TestOrders.Create(Guid.NewGuid());
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTestOrder);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}