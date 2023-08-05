namespace PeakLims.Domain.Panels.Features;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrders.Services;
using Tests.Services;

public static class RemoveTestFromPanel
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid PanelId;
        public readonly Guid TestId;

        public Command(Guid panelId, Guid testId)
        {
            PanelId = panelId;
            TestId = testId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IPanelRepository _panelRepository;
        private readonly ITestRepository _testRepository;
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IPanelRepository panelRepository, ITestRepository testRepository, IHeimGuardClient heimGuard, ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork)
        {
            _testRepository = testRepository;
            _panelRepository = panelRepository;
            _heimGuard = heimGuard;
            _testOrderRepository = testOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemoveTestFromPanel);

            var panel = await _panelRepository.GetById(request.PanelId, true, cancellationToken);
            var test = await _testRepository.GetById(request.TestId, true, cancellationToken);
            panel.RemoveTest(test, _testOrderRepository);
            await _unitOfWork.CommitChanges(cancellationToken);
            
            return true;
        }
    }
}