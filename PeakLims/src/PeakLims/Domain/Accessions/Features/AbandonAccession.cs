namespace PeakLims.Domain.Accessions.Features;

using Databases;
using PeakLims.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class AbandonAccession
{
    public sealed record Command(Guid AccessionId, string Reason) : IRequest;

    public sealed class Handler(
        PeakLimsDbContext dbContext,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accessionToUpdate = (await dbContext.GetAccessionAggregate()
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken))
                .MustBeFoundOrThrow();
            
            accessionToUpdate.Abandon(request.Reason);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}