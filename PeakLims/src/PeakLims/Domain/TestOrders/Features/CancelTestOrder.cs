namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrderCancellationReasons;

public static class CancelTestOrder
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid TestOrderId;
        public readonly string Reason;
        public readonly string Comments;

        public Command(Guid testOrderId, string reason, string comments)
        {
            TestOrderId = testOrderId;
            Reason = reason;
            Comments = comments;
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
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanCancelTestOrders);

            var testOrder = await _testOrderRepository.GetById(request.TestOrderId, cancellationToken: cancellationToken);
            testOrder.Cancel(TestOrderCancellationReason.Of(request.Reason), request.Comments);
            _testOrderRepository.Update(testOrder);

            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}