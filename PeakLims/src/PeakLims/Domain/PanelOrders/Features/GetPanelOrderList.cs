namespace PeakLims.Domain.PanelOrders.Features;

using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.Domain.PanelOrders.Services;
using PeakLims.Exceptions;
using PeakLims.Resources;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;
using QueryKit;
using QueryKit.Configuration;

public static class GetPanelOrderList
{
    public sealed record Query(PanelOrderParametersDto QueryParameters) : IRequest<PagedList<PanelOrderDto>>;

    public sealed class Handler : IRequestHandler<Query, PagedList<PanelOrderDto>>
    {
        private readonly IPanelOrderRepository _panelOrderRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelOrderRepository panelOrderRepository, IHeimGuardClient heimGuard)
        {
            _panelOrderRepository = panelOrderRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PagedList<PanelOrderDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var collection = _panelOrderRepository.Query().AsNoTracking();

            var queryKitConfig = new CustomQueryKitConfiguration();
            var queryKitData = new QueryKitData()
            {
                Filters = request.QueryParameters.Filters,
                SortOrder = request.QueryParameters.SortOrder,
                Configuration = queryKitConfig
            };
            var appliedCollection = collection.ApplyQueryKit(queryKitData);
            var dtoCollection = appliedCollection.ToPanelOrderDtoQueryable();

            return await PagedList<PanelOrderDto>.CreateAsync(dtoCollection,
                request.QueryParameters.PageNumber,
                request.QueryParameters.PageSize,
                cancellationToken);
        }
    }
}