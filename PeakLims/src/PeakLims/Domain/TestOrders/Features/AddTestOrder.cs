namespace PeakLims.Domain.TestOrders.Features;

using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain.TestOrders;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddTestOrder
{
    public sealed class Command : IRequest<TestOrderDto>
    {
        public readonly TestOrderForCreationDto TestOrderToAdd;

        public Command(TestOrderForCreationDto testOrderToAdd)
        {
            TestOrderToAdd = testOrderToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, TestOrderDto>
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

        public async Task<TestOrderDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddTestOrders);

            var testOrderToAdd = request.TestOrderToAdd.ToTestOrderForCreation();
            var testOrder = TestOrder.Create(testOrderToAdd);

            await _testOrderRepository.Add(testOrder, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return testOrder.ToTestOrderDto();
        }
    }
}