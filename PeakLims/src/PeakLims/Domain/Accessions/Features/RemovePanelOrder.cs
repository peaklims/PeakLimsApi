namespace PeakLims.Domain.Accessions.Features;

using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Services;
using SharedKernel.Exceptions;

public static class RemovePanelOrder
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid PanelId;

        public Command(Guid panelId, Guid accessionId)
        {
            PanelId = panelId;
            AccessionId = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IAccessionRepository _accessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IAccessionRepository accessionRepository)
        {
            _panelRepository = panelRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _accessionRepository = accessionRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemovePanelOrders);

            var panel = await _panelRepository.GetById(request.PanelId, false, cancellationToken);
            var accession = await _accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            accession.RemovePanel(panel);

            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}