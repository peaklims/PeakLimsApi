namespace PeakLims.Domain.TestOrders.Features;

using Exceptions;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using PeakLims.Services;
using TestOrderCancellationReasons;

public static class CancelTestOrder
{
    public sealed record Command(Guid TestOrderId, string Reason, string Comments) : IRequest;

    public sealed class Handler(
        ITestOrderRepository testOrderRepository,
        IHeimGuardClient heimGuard,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanCancelTestOrders);

            var testOrder = await testOrderRepository.GetById(request.TestOrderId, cancellationToken: cancellationToken);
            testOrder.Cancel(TestOrderCancellationReason.Of(request.Reason), request.Comments);
            testOrderRepository.Update(testOrder);

            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}