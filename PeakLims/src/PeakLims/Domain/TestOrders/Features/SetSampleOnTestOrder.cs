namespace PeakLims.Domain.TestOrders.Features;

using Databases;
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

    public sealed class Handler(ITestOrderRepository testOrderRepository,
            PeakLimsDbContext dbContext,
            ISampleRepository sampleRepository)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accessions = dbContext.GetAccessionAggregate();
            var accession =
                accessions.FirstOrDefault(x => x.TestOrders.Any(y => y.Id == request.TestOrderId));
            accession.MustBeFoundOrThrow();
            
            var testOrder = accession!.TestOrders
                .FirstOrDefault(x => x.Id == request.TestOrderId)
                .MustBeFoundOrThrow();
            
            if (request.SampleId == null)
            {
                testOrder.RemoveSample();
                testOrderRepository.Update(testOrder);
                await dbContext.SaveChangesAsync(cancellationToken);
                return;
            }
            
            var sample = await sampleRepository.GetById((Guid)request.SampleId, cancellationToken: cancellationToken);
            testOrder.SetSample(sample);
            testOrderRepository.Update(testOrder);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}