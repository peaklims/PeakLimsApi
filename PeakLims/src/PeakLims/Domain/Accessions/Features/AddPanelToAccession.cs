namespace PeakLims.Domain.Accessions.Features;

using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Services;
using SharedKernel.Exceptions;
using TestOrders.Services;
using Tests.Services;

public static class AddPanelToAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid PanelId;
        public readonly Guid AccessionId;

        public Command(Guid accessionId, Guid panelId)
        {
            AccessionId = accessionId;
            PanelId = panelId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly IAccessionRepository _accessionRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IPanelRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IAccessionRepository accessionRepository, ITestOrderRepository testOrderRepository)
        {
            _panelRepository = panelRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _accessionRepository = accessionRepository;
            _testOrderRepository = testOrderRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddPanelToAccession);

            var panel = await _panelRepository.GetById(request.PanelId, true, cancellationToken);
            var accession = await _accessionRepository.GetWithTestOrderWithChildren(request.AccessionId, true, cancellationToken);
            var existingTestOrders = accession.TestOrders.ToList();
            accession.AddPanel(panel);

            await _testOrderRepository.AddRange(accession.TestOrders.Except(existingTestOrders), cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}