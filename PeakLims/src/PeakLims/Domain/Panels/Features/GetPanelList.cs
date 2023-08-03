namespace PeakLims.Domain.Panels.Features;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
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

public static class GetPanelList
{
    public sealed class Query : IRequest<PagedList<PanelDto>>
    {
        public readonly PanelParametersDto QueryParameters;

        public Query(PanelParametersDto queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }

    public sealed class Handler : IRequestHandler<Query, PagedList<PanelDto>>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<PanelDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadPanels);
            
            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder ?? "-CreatedOn",
                Configuration = queryKitConfig
            };

            var collection = _panelRepository.Query().AsNoTracking();
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPanelDtoQueryable();

            return await PagedList<PanelDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}