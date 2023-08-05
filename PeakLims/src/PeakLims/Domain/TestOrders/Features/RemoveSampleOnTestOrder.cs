namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using Samples.Services;
using TestOrderCancellationReasons;

public static class RemoveSampleOnTestOrder
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid TestOrderId;

        public Command(Guid testOrderId)
        {
            TestOrderId = testOrderId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITestOrderRepository testOrderRepository, IHeimGuardClient heimGuard, IUnitOfWork unitOfWork)
        {
            _testOrderRepository = testOrderRepository;
            _heimGuard = heimGuard;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanRemoveSampleOnTestOrder);

            var testOrder = await _testOrderRepository.GetById(request.TestOrderId, cancellationToken: cancellationToken);
            testOrder.RemoveSample();
            _testOrderRepository.Update(testOrder);

            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}