namespace PeakLims.Domain.PanelOrders.Features;

using PeakLims.Domain.PanelOrders.Services;
using PeakLims.Services;
using PeakLims.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeletePanelOrder
{
    public sealed record Command(Guid PanelOrderId) : IRequest;

    public sealed class Handler(
        IPanelOrderRepository panelOrderRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recordToDelete = await panelOrderRepository.GetById(request.PanelOrderId, cancellationToken: cancellationToken);
            panelOrderRepository.Remove(recordToDelete);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}