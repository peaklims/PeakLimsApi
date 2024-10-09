namespace PeakLims.Domain.RolePermissions.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Databases;
using PeakLims.Domain.RolePermissions;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Domain.RolePermissions.Mappings;

public static class AddRolePermission
{
    public sealed record Command(RolePermissionForCreationDto RolePermissionToAdd) : IRequest<RolePermissionDto>;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command, RolePermissionDto>
    {
        public async Task<RolePermissionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddRolePermissions);
            
            var rolePermissionToAdd = request.RolePermissionToAdd.ToRolePermissionForCreation();
            var rolePermission = RolePermission.Create(rolePermissionToAdd);

            await dbContext.RolePermissions.AddAsync(rolePermission, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return rolePermission.ToRolePermissionDto();
        }
    }
}