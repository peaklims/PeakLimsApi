namespace PeakLims.Domain.Containers.Features;

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
using Microsoft.EntityFrameworkCore;
using QueryKit;
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
            
            var collection = _containerRepository.Query().AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToContainerDtoQueryable();

            return await PagedList<ContainerDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}