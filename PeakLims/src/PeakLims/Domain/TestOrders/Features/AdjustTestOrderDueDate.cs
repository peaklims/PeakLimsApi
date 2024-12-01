namespace PeakLims.Domain.TestOrders.Features;

using Databases;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Mappings;

public static class AdjustTestOrderDueDate
{
    public sealed record Command(Guid TestOrderId, DateOnly DueDate) : IRequest;

    public sealed class Handler(ITestOrderRepository testOrderRepository, PeakLimsDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accessions = dbContext.GetAccessionAggregate();
            var accession =
                accessions.FirstOrDefault(x => x.TestOrders.Any(y => y.Id == request.TestOrderId));
            accession.MustBeFoundOrThrow();
            
            var testOrder = accession!.TestOrders
                .FirstOrDefault(x => x.Id == request.TestOrderId)
                .MustBeFoundOrThrow();
            
            testOrder.AdjustDueDate(request.DueDate);
            testOrderRepository.Update(testOrder);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
