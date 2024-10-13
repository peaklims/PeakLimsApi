namespace PeakLims.Domain.PeakOrganizations.Features;

using PeakLims.Domain.PeakOrganizations.Dtos;
using PeakLims.Databases;
using PeakLims.Exceptions;
using PeakLims.Resources;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetPeakOrganizationList
{
    public sealed record Query(PeakOrganizationParametersDto QueryParameters) : IRequest<PagedList<PeakOrganizationDto>>;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Query, PagedList<PeakOrganizationDto>>
    {
        public async Task<PagedList<PeakOrganizationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = dbContext.PeakOrganizations.AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPeakOrganizationDtoQueryable();

            return await PagedList<PeakOrganizationDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}