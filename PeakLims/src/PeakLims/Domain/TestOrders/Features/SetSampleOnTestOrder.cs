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

public static class SetSampleOnTestOrder
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid TestOrderId;
        public readonly Guid SampleId;

        public Command(Guid testOrderId, Guid sampleId)
        {
            TestOrderId = testOrderId;
            SampleId = sampleId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly ISampleRepository _sampleRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITestOrderRepository testOrderRepository, IHeimGuardClient heimGuard, IUnitOfWork unitOfWork, ISampleRepository sampleRepository)
        {
            _testOrderRepository = testOrderRepository;
            _heimGuard = heimGuard;
            _unitOfWork = unitOfWork;
            _sampleRepository = sampleRepository;
        }
        
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanSetSampleOnTestOrder);

            var testOrder = await _testOrderRepository.GetById(request.TestOrderId, cancellationToken: cancellationToken);
            var sample = await _sampleRepository.GetById(request.SampleId, cancellationToken: cancellationToken);
            testOrder.SetSample(sample);
            _testOrderRepository.Update(testOrder);

            await _unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}