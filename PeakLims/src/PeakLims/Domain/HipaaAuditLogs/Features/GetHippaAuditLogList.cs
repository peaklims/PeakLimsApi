namespace PeakLims.Domain.HipaaAuditLogs.Features;

using PeakLims.Domain.HipaaAuditLogs.Dtos;
using PeakLims.Domain.HipaaAuditLogs.Services;
using PeakLims.Exceptions;
using PeakLims.Resources;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;
using Wrappers;

public static class GetHipaaAuditLogList
{
    public sealed record Query(HipaaAuditLogParametersDto QueryParameters) : IRequest<PagedList<HipaaAuditLogDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<HipaaAuditLogDto>>
    {
        private readonly IHipaaAuditLogRepository _hipaaAuditLogRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHipaaAuditLogRepository hipaaAuditLogRepository, IHeimGuardClient heimGuard)
        {
            _hipaaAuditLogRepository = hipaaAuditLogRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<HipaaAuditLogDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = _hipaaAuditLogRepository.Query().AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
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