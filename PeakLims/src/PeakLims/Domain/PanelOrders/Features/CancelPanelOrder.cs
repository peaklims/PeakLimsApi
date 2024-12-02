namespace PeakLims.Domain.PanelOrders.Features;

using Databases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services;
using TestOrderCancellationReasons;

public static class CancelPanelOrder
{
    public sealed record Command(Guid PanelOrderId, string Reason, string Comments) : IRequest;

    public sealed class Handler(PeakLimsDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accessionId = dbContext.PanelOrders
                .Include(x => x.TestOrders)
                .ThenInclude(x => x.Accession)
                .Where(x => x.Id == request.PanelOrderId)
                .AsNoTracking()
                .Select(x => x.TestOrders)
                .FirstOrDefault()
                ?.FirstOrDefault()
                ?.Accession
                ?.MustBeFoundOrThrow()
                .Id;

            var accession = await dbContext.GetAccessionAggregate()
                .GetById((Guid)accessionId!, cancellationToken: cancellationToken);
            accession.MustBeFoundOrThrow();

            var panelOrder = accession
                .TestOrders
                .FirstOrDefault(x => x.PanelOrder.Id == request.PanelOrderId)
                .MustBeFoundOrThrow()
                .PanelOrder;
            
            panelOrder.Cancel(TestOrderCancellationReason.Of(request.Reason), request.Comments);
            dbContext.PanelOrders.Update(panelOrder);

            await  dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
