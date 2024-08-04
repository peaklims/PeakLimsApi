namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using PanelOrders.Services;
using TestOrders.Services;

public static class RemovePanelOrderFromAccession
{
    public sealed record Command(Guid AccessionId, Guid PanelOrderId) : IRequest;

    public sealed class Handler(IUnitOfWork unitOfWork, IHeimGuardClient heimGuard,
            IAccessionRepository accessionRepository, IPanelOrderRepository panelOrderRepository,
            ITestOrderRepository testOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var panelOrderToRemove = await panelOrderRepository.GetById(request.PanelOrderId, true, cancellationToken);
            accession.RemovePanelOrder(panelOrderToRemove);
            
            panelOrderRepository.Remove(panelOrderToRemove);
            testOrderRepository.RemoveRange(panelOrderToRemove.TestOrders);
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}