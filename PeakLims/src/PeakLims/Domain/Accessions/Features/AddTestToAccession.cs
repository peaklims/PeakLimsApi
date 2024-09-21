namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Services;
using Services;
using TestOrders.Services;
using Tests.Services;

public static class AddTestToAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid TestId;

        public Command( Guid accessionId, Guid testId)
        {
            AccessionId = accessionId;
            TestId = testId;
        }
    }

    public sealed class Handler(
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard,
        ITestRepository testRepository,
        PeakLimsDbContext dbContext,
        ITestOrderRepository testOrderRepository)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = (await dbContext.GetAccessionAggregate()
                    .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            var testToAdd = await testRepository.GetById(request.TestId, true, cancellationToken);
            var existingTestOrders = accession.TestOrders.ToList();
            accession.AddTest(testToAdd);

            await testOrderRepository.AddRange(accession.TestOrders.Except(existingTestOrders), cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);
            return true;
        }
    }
}