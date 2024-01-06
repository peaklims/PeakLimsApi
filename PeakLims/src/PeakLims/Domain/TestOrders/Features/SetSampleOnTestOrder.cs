namespace PeakLims.Domain.TestOrders.Features;

using Exceptions;
using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services;
using Samples.Services;
using TestOrderCancellationReasons;

public static class SetSampleOnTestOrder
{
    public sealed record Command(Guid TestOrderId, Guid? SampleId) : IRequest;

    public sealed class Handler(ITestOrderRepository testOrderRepository, IHeimGuardClient heimGuard,
            IUnitOfWork unitOfWork, ISampleRepository sampleRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanSetSampleOnTestOrder);
            var testOrder = await testOrderRepository.Query()
                .Include(x => x.Sample)
                .FirstOrDefaultAsync(x => x.Id == request.TestOrderId, cancellationToken: cancellationToken);
            
            if (testOrder == null)
                throw new NotFoundException(nameof(TestOrder), request.TestOrderId);
            
            if (request.SampleId == null)
            {
                testOrder.RemoveSample();
                testOrderRepository.Update(testOrder);
                await unitOfWork.CommitChanges(cancellationToken);
                return;
            }
            
            var sample = await sampleRepository.GetById((Guid)request.SampleId, cancellationToken: cancellationToken);
            testOrder.SetSample(sample);
            testOrderRepository.Update(testOrder);

            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}