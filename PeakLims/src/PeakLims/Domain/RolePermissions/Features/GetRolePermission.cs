namespace PeakLims.Domain.RolePermissions.Features;

using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Databases;
using PeakLims.Domain;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Domain.RolePermissions.Mappings;
using PeakLims.Exceptions;

public static class GetRolePermission
{
    public sealed record Query(Guid RolePermissionId) : IRequest<RolePermissionDto>;

    public sealed class Handler(PeakLimsDbContext dbContext, IHeimGuardClient heimGuard)
        : IRequestHandler<Query, RolePermissionDto>
    {
        public async Task<RolePermissionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadRolePermissions);

            var result = await dbContext.RolePermissions
                .AsNoTracking()
                .GetById(request.RolePermissionId, cancellationToken);
            return result.ToRolePermissionDto();
        }
    }
}