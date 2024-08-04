namespace PeakLims.Domain.Accessions.Features;

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
            IAccessionRepository accessionRepository, IPanelOrderRepository panelOrderRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            if (accession == null)
            {
                throw new NotFoundException($"Accession with id {request.AccessionId} not found.");
            }
            var panel = await panelRepository.GetById(request.PanelId, true, cancellationToken);
            var panelOrder = accession.AddPanel(panel);

            await panelOrderRepository.Add(panelOrder, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}