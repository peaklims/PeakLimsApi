namespace PeakLims.IntegrationTests.FeatureTests.RolePermissions;

using PeakLims.SharedTestHelpers.Fakes.RolePermission;
using PeakLims.Domain.RolePermissions.Features;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using SharedKernel.Exceptions;
using System.Threading.Tasks;

public class RolePermissionQueryTests : TestBase
{
    [Fact]
    public async Task can_get_existing_rolepermission_with_accurate_props()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRolePermissionOne = new FakeRolePermissionBuilder().Build();
        await testingServiceScope.InsertAsync(fakeRolePermissionOne);

        // Act
        var query = new GetRolePermission.Query(fakeRolePermissionOne.Id);
        var rolePermission = await testingServiceScope.SendAsync(query);

        // Assert
        rolePermission.Permission.Should().Be(fakeRolePermissionOne.Permission);
        rolePermission.Role.Should().Be(fakeRolePermissionOne.Role.Value);
    }

    [Fact]
    public async Task get_rolepermission_throws_notfound_exception_when_record_does_not_exist()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var badId = Guid.NewGuid();

        // Act
        var query = new GetRolePermission.Query(badId);
        Func<Task> act = () => testingServiceScope.SendAsync(query);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}