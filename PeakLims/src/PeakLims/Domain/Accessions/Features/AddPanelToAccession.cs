namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PanelOrders.Services;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Services;
using TestOrders.Services;
using Tests.Services;

public static class AddPanelToAccession
{
    public sealed record Command(Guid AccessionId, Guid PanelId) : IRequest;

    public sealed class Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard,
            PeakLimsDbContext dbContext, IPanelOrderRepository panelOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                    .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var panel = await panelRepository.GetById(request.PanelId, true, cancellationToken);
            var panelOrder = accession.AddPanel(panel);

            await panelOrderRepository.Add(panelOrder, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}