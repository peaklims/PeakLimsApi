namespace PeakLims.FunctionalTests.FunctionalTests.Users;

using PeakLims.SharedTestHelpers.Fakes.User;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class DeleteUserTests : TestBase
{
    [Fact]
    public async Task delete_user_returns_nocontent_when_entity_exists_and_auth_credentials_are_valid()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);
        await InsertAsync(fakeUser);

        // Act
        var route = ApiRoutes.Users.Delete(fakeUser.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
            
    [Fact]
    public async Task delete_user_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();

        // Act
        var route = ApiRoutes.Users.Delete(fakeUser.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task delete_user_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeUser = new FakeUserBuilder().Build();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.Users.Delete(fakeUser.Id);
        var result = await FactoryClient.DeleteRequestAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}