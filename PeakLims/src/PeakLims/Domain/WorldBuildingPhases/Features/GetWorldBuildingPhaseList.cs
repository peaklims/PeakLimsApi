namespace PeakLims.Domain.WorldBuildingPhases.Features;

using PeakLims.Domain.WorldBuildingPhases.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using PeakLims.Resources;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetWorldBuildingPhaseList
{
    public sealed record Query(WorldBuildingPhaseParametersDto QueryParameters) : IRequest<PagedList<WorldBuildingPhaseDto>>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<WorldBuildingPhaseDto>>
    {
        public async Task<PagedList<WorldBuildingPhaseDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.WorldBuildingPhases.AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToWorldBuildingPhaseDtoQueryable();

            return await PagedList<WorldBuildingPhaseDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}