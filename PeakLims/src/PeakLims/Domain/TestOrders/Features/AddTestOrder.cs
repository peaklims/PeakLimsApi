namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Panels.Services;
using Tests.Services;

public static class AddTestOrder
{
    public sealed record Command(Guid TestId, Guid? PanelId) : IRequest<TestOrderDto>;

    public sealed class Handler : IRequestHandler<Command, TestOrderDto>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;
        private readonly ITestRepository _testRepository;
        private readonly IPanelRepository _panelRepository;

        public Handler(ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, ITestRepository testRepository, IPanelRepository panelRepository)
        {
            _testOrderRepository = testOrderRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _testRepository = testRepository;
            _panelRepository = panelRepository;
        }

        public async Task<TestOrderDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTestOrders);

            var test = await _testRepository.GetById(request.TestId, true, cancellationToken);
            var testOrder = TestOrder.Create(test);
            if(request.PanelId.HasValue)
            {
                var panel = await _panelRepository.GetById(request.PanelId.Value, true, cancellationToken);
                testOrder = TestOrder.Create(test, panel);
            }

            await _testOrderRepository.Add(testOrder, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return testOrder.ToTestOrderDto();
        }
    }
}