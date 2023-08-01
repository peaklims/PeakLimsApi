namespace PeakLims.FunctionalTests.FunctionalTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateTestTests : TestBase
{
    [Fact]
    public async Task create_test_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakeTest = new FakeTestForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.Tests.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_test_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeTest = new FakeTestForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.Tests.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_test_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeTest = new FakeTestForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Tests.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeTest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}