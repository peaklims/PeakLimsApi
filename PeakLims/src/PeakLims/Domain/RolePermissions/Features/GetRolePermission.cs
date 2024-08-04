namespace PeakLims.Domain.RolePermissions.Features;

using Exceptions;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Domain.RolePermissions.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetRolePermission
{
    public sealed class Query : IRequest<RolePermissionDto>
    {
        public readonly Guid Id;

        public Query(Guid id)
        {
            Id = id;
        }
    }

    public sealed class Handler : IRequestHandler<Query, RolePermissionDto>
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IRolePermissionRepository rolePermissionRepository, IHeimGuardClient heimGuard)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _heimGuard = heimGuard;
        }

        public async Task<RolePermissionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _rolePermissionRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToRolePermissionDto();
        }
    }
}