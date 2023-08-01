namespace PeakLims.Domain.RolePermissions.Features;

using Ardalis.Specification;
using PeakLims.Domain.RolePermissions.Dtos;
using PeakLims.Domain.RolePermissions.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetRolePermissionList
{
    public sealed class Query : IRequest<PagedList<RolePermissionDto>>
    {
        public readonly RolePermissionParametersDto QueryParameters;

        public Query(RolePermissionParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<RolePermissionDto>>
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IRolePermissionRepository rolePermissionRepository, IHeimGuardClient heimGuard)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<RolePermissionDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadRolePermissions);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var rolePermissionSpecification = new RolePermissionWorklistSpecification(request.QueryParameters, queryKitData);
            var rolePermissionList = await _rolePermissionRepository.ListAsync(rolePermissionSpecification, cancellationToken);
            var totalRolePermissionCount = await _rolePermissionRepository.TotalCount(cancellationToken);

            return new PagedList<RolePermissionDto>(rolePermissionList, 
                totalRolePermissionCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class RolePermissionWorklistSpecification : Specification<RolePermission, RolePermissionDto>
        {
            public RolePermissionWorklistSpecification(RolePermissionParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToRolePermissionDto())
                    .AsNoTracking();
            }
        }
    }
}