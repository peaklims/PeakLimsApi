namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using PanelOrders.Services;
using TestOrders.Services;

public static class RemovePanelOrderFromAccession
{
    public sealed record Command(Guid AccessionId, Guid PanelOrderId) : IRequest;

    public sealed class Handler(IUnitOfWork unitOfWork, 
            IHeimGuardClient heimGuard,
            PeakLimsDbContext dbContext, 
            IPanelOrderRepository panelOrderRepository,
            ITestOrderRepository testOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                    .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var panelOrderToRemove = await panelOrderRepository.GetById(request.PanelOrderId, true, cancellationToken);
            accession.RemovePanelOrder(panelOrderToRemove);
            
            panelOrderRepository.Remove(panelOrderToRemove);
            testOrderRepository.RemoveRange(panelOrderToRemove.TestOrders);
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}