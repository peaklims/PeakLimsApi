namespace PeakLims.IntegrationTests.FeatureTests.RolePermissions;

using PeakLims.SharedTestHelpers.Fakes.RolePermission;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using PeakLims.Domain.RolePermissions.Features;
using SharedKernel.Exceptions;

public class AddRolePermissionCommandTests : TestBase
{
    [Fact]
    public async Task can_add_new_rolepermission_to_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRolePermissionOne = new FakeRolePermissionForCreationDto().Generate();

        // Act
        var command = new AddRolePermission.Command(fakeRolePermissionOne);
        var rolePermissionReturned = await testingServiceScope.SendAsync(command);
        var rolePermissionCreated = await testingServiceScope.ExecuteDbContextAsync(db => db.RolePermissions
            .FirstOrDefaultAsync(r => r.Id == rolePermissionReturned.Id));

        // Assert
        rolePermissionReturned.Permission.Should().Be(fakeRolePermissionOne.Permission);
        rolePermissionReturned.Role.Should().Be(fakeRolePermissionOne.Role);

        rolePermissionCreated?.Permission.Should().Be(fakeRolePermissionOne.Permission);
        rolePermissionCreated?.Role.Value.Should().Be(fakeRolePermissionOne.Role);
    }
}