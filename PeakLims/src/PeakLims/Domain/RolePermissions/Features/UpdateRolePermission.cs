namespace PeakLims.Domain.RolePermissions.Features;

using Databases;
using Dtos;
using Exceptions;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateRolePermission
{
    public sealed record Command(Guid RolePermissionId, RolePermissionForUpdateDto UpdatedRolePermissionData) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateRolePermissions);

            var rolePermissionToUpdate = await dbContext.RolePermissions.GetById(request.RolePermissionId, cancellationToken);
            var rolePermissionToAdd = request.UpdatedRolePermissionData.ToRolePermissionForUpdate();
            rolePermissionToUpdate.Update(rolePermissionToAdd);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}