namespace PeakLims.Domain.PanelOrders.Features;

using PeakLims.Domain.PanelOrders.Services;
using PeakLims.Domain.PanelOrders;
using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.Domain.PanelOrders.Models;
using PeakLims.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddPanelOrder
{
    public sealed record Command(PanelOrderForCreationDto PanelOrderToAdd) : IRequest<PanelOrderDto>;

    public sealed class Handler : IRequestHandler<Command, PanelOrderDto>
    {
        private readonly IPanelOrderRepository _panelOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelOrderRepository panelOrderRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _panelOrderRepository = panelOrderRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<PanelOrderDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddPanelOrders);

            var panelOrderToAdd = request.PanelOrderToAdd.ToPanelOrderForCreation();
            var panelOrder = PanelOrder.Create(panelOrderToAdd);

            await _panelOrderRepository.Add(panelOrder, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return panelOrder.ToPanelOrderDto();
        }
    }
}