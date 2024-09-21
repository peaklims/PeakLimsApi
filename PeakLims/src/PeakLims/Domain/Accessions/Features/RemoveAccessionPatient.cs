namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using Patients.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class RemoveAccessionPatient
{
    public sealed record Command(Guid AccessionId) : IRequest;

    public sealed class Handler(
        IAccessionRepository accessionRepository,
        IUnitOfWork unitOfWork,
        PeakLimsDbContext dbContext, 
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await dbContext.GetAccessionAggregate()
                .GetById(request.AccessionId, cancellationToken: cancellationToken);
            
            accession.MustBeFoundOrThrow();
            accession.RemovePatient();

            accessionRepository.Update(accession);
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}