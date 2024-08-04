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

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var recordToDelete = await _panelOrderRepository.GetById(request.PanelOrderId, cancellationToken: cancellationToken);
            _panelOrderRepository.Remove(recordToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}