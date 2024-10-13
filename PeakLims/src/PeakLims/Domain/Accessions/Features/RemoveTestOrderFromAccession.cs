namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using TestOrders.Services;

public static class RemoveTestOrderFromAccession
{
    public sealed record Command(Guid AccessionId, Guid TestOrderId) : IRequest;

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        PeakLimsDbContext dbContext,
        ITestOrderRepository testOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                    .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var testOrderToRemove = await testOrderRepository.GetById(request.TestOrderId, true, cancellationToken);
            accession.RemoveTestOrder(testOrderToRemove);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}