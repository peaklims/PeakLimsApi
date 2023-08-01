namespace PeakLims.Domain.RolePermissions;

using Dtos;
using DomainEvents;
using Roles;
using Domain;
using SharedKernel.Exceptions;
using PeakLims.Domain.RolePermissions.Models;

public class RolePermission : BaseEntity
{
    public Role Role { get; private set; }
    public string Permission { get; private set; }

    // Add Props Marker -- Deleting this comment will cause the add props utility to be incomplete


    public static RolePermission Create(RolePermissionForCreation rolePermissionForCreation)
    {
        ValidationException.Must(BeAnExistingPermission(rolePermissionForCreation.Permission), 
            "Please use a valid permission.");

        var newRolePermission = new RolePermission();

        newRolePermission.Role = new Role(rolePermissionForCreation.Role);
        newRolePermission.Permission = rolePermissionForCreation.Permission;

        newRolePermission.QueueDomainEvent(new RolePermissionCreated(){ RolePermission = newRolePermission });
        
        return newRolePermission;
    }

    public RolePermission Update(RolePermissionForUpdate rolePermissionForUpdate)
    {
        ValidationException.Must(BeAnExistingPermission(rolePermissionForUpdate.Permission), 
            "Please use a valid permission.");

        Role = new Role(rolePermissionForUpdate.Role);
        Permission = rolePermissionForUpdate.Permission;

        QueueDomainEvent(new RolePermissionUpdated(){ Id = Id });
        return this;
    }

    // Add Prop Methods Marker -- Deleting this comment will cause the add props utility to be incomplete
    
    private static bool BeAnExistingPermission(string permission)
    {
        return Permissions.List().Contains(permission, StringComparer.InvariantCultureIgnoreCase);
    }
    
    protected RolePermission() { } // For EF + Mocking
}