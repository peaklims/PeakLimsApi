namespace PeakLims.Domain.WorldBuildingAttempts.Features;

using PeakLims.Domain.WorldBuildingAttempts.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using PeakLims.Resources;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetWorldBuildingAttemptList
{
    public sealed record Query(WorldBuildingAttemptParametersDto QueryParameters) : IRequest<PagedList<WorldBuildingAttemptDto>>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<WorldBuildingAttemptDto>>
    {
        public async Task<PagedList<WorldBuildingAttemptDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.WorldBuildingAttempts.AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToWorldBuildingAttemptDtoQueryable();

            return await PagedList<WorldBuildingAttemptDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}