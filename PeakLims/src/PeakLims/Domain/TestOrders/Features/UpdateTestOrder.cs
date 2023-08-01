namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Services;
using PeakLims.Domain.TestOrders.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateTestOrder
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly TestOrderForUpdateDto UpdatedTestOrderData;

        public Command(Guid id, TestOrderForUpdateDto updatedTestOrderData)
        {
            Id = id;
            UpdatedTestOrderData = updatedTestOrderData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(ITestOrderRepository testOrderRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _testOrderRepository = testOrderRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateTestOrders);

            var testOrderToUpdate = await _testOrderRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var testOrderToAdd = request.UpdatedTestOrderData.ToTestOrderForUpdate();
            testOrderToUpdate.Update(testOrderToAdd);

            _testOrderRepository.Update(testOrderToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}