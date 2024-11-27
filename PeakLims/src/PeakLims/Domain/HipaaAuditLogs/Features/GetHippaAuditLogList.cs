namespace PeakLims.Domain.HipaaAuditLogs.Features;

using Databases;
using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Resources;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetHipaaAuditLogList
{
    public sealed record Query(HipaaAuditLogParametersDto QueryParameters) : IRequest<PagedList<HipaaAuditLogDto>>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<HipaaAuditLogDto>>
    {

        public async Task<PagedList<HipaaAuditLogDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.HipaaAuditLogs.AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration(config =>
            {
            });
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToHipaaAuditLogDtoQueryable();

            return await PagedList<HipaaAuditLogDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}