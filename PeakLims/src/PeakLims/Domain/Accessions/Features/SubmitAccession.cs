namespace PeakLims.Domain.Accessions.Features;

using Databases;
using PeakLims.Services;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class SubmitAccession
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;

        public Command(Guid accessionId)
        {
            AccessionId = accessionId;
        }
    }

    public sealed class Handler(
        PeakLimsDbContext dbContext,
        IUnitOfWork unitOfWork,
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var accessionToUpdate = (await dbContext.GetAccessionAggregate()
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            accessionToUpdate.Submit();
            return await unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}