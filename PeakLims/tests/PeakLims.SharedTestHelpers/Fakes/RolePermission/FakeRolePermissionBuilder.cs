namespace PeakLims.SharedTestHelpers.Fakes.RolePermission;

using PeakLims.Domain.RolePermissions;
using PeakLims.Domain.RolePermissions.Models;

public class FakeRolePermissionBuilder
{
    private RolePermissionForCreation _creationData = new FakeRolePermissionForCreation().Generate();

    public FakeRolePermissionBuilder WithModel(RolePermissionForCreation model)
    {
        _creationData = model;
        return this;
    }
    
    public FakeRolePermissionBuilder WithRole(string role)
    {
        _creationData.Role = role;
        return this;
    }
    
    public FakeRolePermissionBuilder WithPermission(string permission)
    {
        _creationData.Permission = permission;
        return this;
    }
    
    public RolePermission Build()
    {
        var result = RolePermission.Create(_creationData);
        return result;
    }
}