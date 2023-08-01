namespace PeakLims.SharedTestHelpers.Fakes.RolePermission;

using AutoBogus;
using PeakLims.Domain;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Domain.Roles;
using PeakLims.Domain.RolePermissions.Models;

public sealed class FakeRolePermissionForUpdateDto : AutoFaker<RolePermissionForUpdateDto>
{
    public FakeRolePermissionForUpdateDto()
    {
        RuleFor(rp => rp.Permission, f => f.PickRandom(Permissions.List()));
        RuleFor(rp => rp.Role, f => f.PickRandom(Role.ListNames()));
    }
}