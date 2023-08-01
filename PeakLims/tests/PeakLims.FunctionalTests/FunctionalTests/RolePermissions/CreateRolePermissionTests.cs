namespace PeakLims.FunctionalTests.FunctionalTests.RolePermissions;

using PeakLims.SharedTestHelpers.Fakes.RolePermission;
using PeakLims.FunctionalTests.TestUtilities;
using PeakLims.Domain;
using SharedKernel.Domain;
using FluentAssertions;
using Xunit;
using System.Net;
using System.Threading.Tasks;

public class CreateRolePermissionTests : TestBase
{
    [Fact]
    public async Task create_rolepermission_returns_created_using_valid_dto_and_valid_auth_credentials()
    {
        // Arrange
        var fakeRolePermission = new FakeRolePermissionForCreationDto().Generate();

        var user = await AddNewSuperAdmin();
        FactoryClient.AddAuth(user.Identifier);

        // Act
        var route = ApiRoutes.RolePermissions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeRolePermission);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
            
    [Fact]
    public async Task create_rolepermission_returns_unauthorized_without_valid_token()
    {
        // Arrange
        var fakeRolePermission = new FakeRolePermissionForCreationDto { }.Generate();

        // Act
        var route = ApiRoutes.RolePermissions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeRolePermission);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
            
    [Fact]
    public async Task create_rolepermission_returns_forbidden_without_proper_scope()
    {
        // Arrange
        var fakeRolePermission = new FakeRolePermissionForCreationDto { }.Generate();
        FactoryClient.AddAuth();

        // Act
        var route = ApiRoutes.RolePermissions.Create;
        var result = await FactoryClient.PostJsonRequestAsync(route, fakeRolePermission);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}