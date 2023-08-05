namespace PeakLims.Domain.Accessions.Features;

using HeimGuard;
using MediatR;
using Panels.Services;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using SharedKernel.Exceptions;
using TestOrders.Services;

public static class RemovePanelFromAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid PanelId;

        public Command( Guid accessionId, Guid panelId)
        {
            AccessionId = accessionId;
            PanelId = panelId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IAccessionRepository accessionRepository, ITestOrderRepository testOrderRepository, IPanelRepository panelRepository)
        {
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _accessionRepository = accessionRepository;
            _testOrderRepository = testOrderRepository;
            _panelRepository = panelRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteTestOrders);

            var accession = await _accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var panel = await _panelRepository.GetById(request.PanelId, true, cancellationToken);
            accession.RemovePanel(panel);
            await _unitOfWork.CommitChanges(cancellationToken);
            
            _testOrderRepository.CleanupOrphanedTestOrders();
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}