namespace PeakLims.Domain.PanelOrders.Features;

using PeakLims.Domain.PanelOrders.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrderCancellationReasons;

public static class CancelPanelOrder
{
    public sealed record Command(Guid PanelOrderId, string Reason, string Comments) : IRequest;

    public sealed class Handler(
        IPanelOrderRepository panelOrderRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var panelOrder = await panelOrderRepository.GetById(request.PanelOrderId, true, cancellationToken);
            panelOrder.Cancel(TestOrderCancellationReason.Of(request.Reason), request.Comments);
            panelOrderRepository.Update(panelOrder);

            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}
