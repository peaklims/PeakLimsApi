namespace PeakLims.Domain.Containers.Features;

using Ardalis.Specification;
using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using QueryKit.Configuration;

public static class GetContainerList
{
    public sealed class Query : IRequest<PagedList<ContainerDto>>
    {
        public readonly ContainerParametersDto QueryParameters;

        public Query(ContainerParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<ContainerDto>>
    {
        private readonly IContainerRepository _containerRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IContainerRepository containerRepository, IHeimGuardClient heimGuard)
        {
            _containerRepository = containerRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<ContainerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadContainers);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var containerSpecification = new ContainerWorklistSpecification(request.QueryParameters, queryKitData);
            var containerList = await _containerRepository.ListAsync(containerSpecification, cancellationToken);
            var totalContainerCount = await _containerRepository.TotalCount(cancellationToken);

            return new PagedList<ContainerDto>(containerList, 
                totalContainerCount,
                request.QueryParameters.PageNumber, 
                request.QueryParameters.PageSize);
        }

        private sealed class ContainerWorklistSpecification : Specification<Container, ContainerDto>
        {
            public ContainerWorklistSpecification(ContainerParametersDto parameters, QueryKitData queryKitData)
            {
                Query.ApplyQueryKit(queryKitData)
                    .Paginate(parameters.PageNumber, parameters.PageSize)
                    .Select(x => x.ToContainerDto())
                    .AsNoTracking();
            }
        }
    }
}