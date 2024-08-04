namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using TestOrders.Services;

public static class RemoveTestOrderFromAccession
{
    public sealed record Command(Guid AccessionId, Guid TestOrderId) : IRequest;

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard,
        IAccessionRepository accessionRepository,
        ITestOrderRepository testOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var testOrderToRemove = await testOrderRepository.GetById(request.TestOrderId, true, cancellationToken);
            accession.RemoveTestOrder(testOrderToRemove);
            await unitOfWork.CommitChanges(cancellationToken);
            
            testOrderRepository.CleanupOrphanedTestOrders();
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}