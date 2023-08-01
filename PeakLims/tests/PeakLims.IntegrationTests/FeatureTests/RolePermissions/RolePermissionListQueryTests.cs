namespace PeakLims.IntegrationTests.FeatureTests.RolePermissions;

using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.SharedTestHelpers.Fakes.RolePermission;
using SharedKernel.Exceptions;
using PeakLims.Domain.RolePermissions.Features;
using FluentAssertions;
using Domain;
using Xunit;
using System.Threading.Tasks;

public class RolePermissionListQueryTests : TestBase
{
    
    [Fact]
    public async Task can_get_rolepermission_list()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        var fakeRolePermissionOne = new FakeRolePermissionBuilder().Build();
        var fakeRolePermissionTwo = new FakeRolePermissionBuilder().Build();
        var queryParameters = new RolePermissionParametersDto();

        await testingServiceScope.InsertAsync(fakeRolePermissionOne, fakeRolePermissionTwo);

        // Act
        var query = new GetRolePermissionList.Query(queryParameters);
        var rolePermissions = await testingServiceScope.SendAsync(query);

        // Assert
        rolePermissions.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task must_be_permitted()
    {
        // Arrange
        var testingServiceScope = new TestingServiceScope();
        testingServiceScope.SetUserNotPermitted(Permissions.CanReadRolePermissions);
        var queryParameters = new RolePermissionParametersDto();

        // Act
        var command = new GetRolePermissionList.Query(queryParameters);
        Func<Task> act = () => testingServiceScope.SendAsync(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}