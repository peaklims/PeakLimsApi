namespace PeakLims.Domain.Panels.Features;

using Exceptions;
using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrders.Services;
using Tests.Services;

public static class AddTestToPanel
{
    public sealed record Command(Guid PanelId, Guid TestId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var panel = await _panelRepository.GetById(request.PanelId, cancellationToken: cancellationToken);
            var test = await _testRepository.GetById(request.TestId, cancellationToken: cancellationToken);
            panel.AddTest(test, _testOrderRepository);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}