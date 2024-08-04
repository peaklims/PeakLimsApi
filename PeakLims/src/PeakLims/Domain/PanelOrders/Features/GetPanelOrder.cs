namespace PeakLims.Domain.PanelOrders.Features;

using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.Domain.PanelOrders.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetPanelOrder
{
    public sealed record Query(Guid PanelOrderId) : IRequest<PanelOrderDto>;

    public sealed class Handler : IRequestHandler<Query, PanelOrderDto>
    {
        private readonly IPanelOrderRepository _panelOrderRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelOrderRepository panelOrderRepository, IHeimGuardClient heimGuard)
        {
            _panelOrderRepository = panelOrderRepository;
            _heimGuard = heimGuard;
        }

        public async Task<PanelOrderDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _panelOrderRepository.GetById(request.PanelOrderId, cancellationToken: cancellationToken);
            return result.ToPanelOrderDto();
        }
    }
}