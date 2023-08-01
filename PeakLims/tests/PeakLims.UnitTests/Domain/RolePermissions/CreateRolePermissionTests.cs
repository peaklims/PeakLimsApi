namespace PeakLims.UnitTests.Domain.RolePermissions;

using SharedKernel.Exceptions;
using PeakLims.Domain;
using PeakLims.Domain.RolePermissions;
using PeakLims.Wrappers;
using PeakLims.Domain.RolePermissions.Models;
using PeakLims.Domain.Roles;
using Bogus;
using FluentAssertions;
using Xunit;
using ValidationException = SharedKernel.Exceptions.ValidationException;

public class CreateRolePermissionTests
{
    private readonly Faker _faker;

    public CreateRolePermissionTests()
    {
        _faker = new Faker();
    }
    
    [Fact]
    public void can_create_valid_rolepermission()
    {
        // Arrange
        var permission = _faker.PickRandom(Permissions.List());
        var role = _faker.PickRandom(Role.ListNames());

        // Act
        var newRolePermission = RolePermission.Create(new RolePermissionForCreation()
        {
            Permission = permission,
            Role = role
        });
        
        // Assert
        newRolePermission.Permission.Should().Be(permission);
        newRolePermission.Role.Value.Should().Be(role);
    }
    
    [Fact]
    public void can_NOT_create_rolepermission_with_invalid_role()
    {
        // Arrange
        var rolePermission = () => RolePermission.Create(new RolePermissionForCreation()
        {
            Permission = _faker.PickRandom(Permissions.List()),
            Role = _faker.Lorem.Word()
        });

        // Act + Assert
        rolePermission.Should().Throw<InvalidSmartEnumPropertyName>();
    }
    
    [Fact]
    public void can_NOT_create_rolepermission_with_invalid_permission()
    {
        // Arrange
        var rolePermission = () => RolePermission.Create(new RolePermissionForCreation()
        {
            Role = _faker.PickRandom(Role.ListNames()),
            Permission = _faker.Lorem.Word()
        });

        // Act + Assert
        rolePermission.Should().Throw<ValidationException>();
    }
}