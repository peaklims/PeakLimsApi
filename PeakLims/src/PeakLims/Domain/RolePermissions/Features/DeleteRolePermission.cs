namespace PeakLims.Domain.RolePermissions.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Databases;

public static class DeleteRolePermission
{
    public sealed record Command(Guid RolePermissionId) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteRolePermissions);
            
            var recordToDelete = await dbContext.RolePermissions.GetById(request.RolePermissionId, cancellationToken: cancellationToken);
            dbContext.Remove(recordToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}