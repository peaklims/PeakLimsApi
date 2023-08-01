namespace PeakLims.IntegrationTests.FeatureTests.RolePermissions;

using PeakLims.SharedTestHelpers.Fakes.RolePermission;
using PeakLims.Domain.RolePermissions.Dtos;
using SharedKernel.Exceptions;
using PeakLims.Domain.RolePermissions.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;

public class UpdateRolePermissionCommandTests : TestBase
{
    [Fact]
    public async Task can_update_existing_rolepermission_in_db()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRolePermissionOne = new FakeRolePermissionBuilder().Build();
        var updatedRolePermissionDto = new FakeRolePermissionForUpdateDto().Generate();
        await testingServiceScope.InsertAsync(fakeRolePermissionOne);

        var rolePermission = await testingServiceScope.ExecuteDbContextAsync(db => db.RolePermissions
            .FirstOrDefaultAsync(r => r.Id == fakeRolePermissionOne.Id));
        var id = rolePermission.Id;

        // Act
        var command = new UpdateRolePermission.Command(id, updatedRolePermissionDto);
        await testingServiceScope.SendAsync(command);
        var updatedRolePermission = await testingServiceScope.ExecuteDbContextAsync(db => db.RolePermissions.FirstOrDefaultAsync(r => r.Id == id));

        // Assert
        updatedRolePermission?.Permission.Should().Be(updatedRolePermissionDto.Permission);
        updatedRolePermission?.Role.Value.Should().Be(updatedRolePermissionDto.Role);
    }
}