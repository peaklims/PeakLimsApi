namespace PeakLims.FunctionalTests.FunctionalTests.Tests;

using PeakLims.SharedTestHelpers.Fakes.Test;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class UpdateTestRecordTests : TestBase
{
    [Fact]
    public async Task put_test_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        var updatedTestDto = new FakeTestForUpdateDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeTest);

        // Act
        var route = ApiRoutes.Tests.Put(fakeTest.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task put_test_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        var updatedTestDto = new FakeTestForUpdateDto { }.Generate();

        // Act
        var route = ApiRoutes.Tests.Put(fakeTest.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task put_test_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeTest = new FakeTestBuilder().Build();
        var updatedTestDto = new FakeTestForUpdateDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Tests.Put(fakeTest.Id);
        var result = await FactoryClient.PutJsonRequestAsync(route, updatedTestDto);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}