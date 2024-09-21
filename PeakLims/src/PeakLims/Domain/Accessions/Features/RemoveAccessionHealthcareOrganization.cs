namespace PeakLims.Domain.Accessions.Features;

using Databases;
using Exceptions;
using HealthcareOrganizations.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class RemoveAccessionHealthcareOrganization
{
    public sealed record Command(Guid AccessionId) : IRequest<bool>;

    public sealed class Handler(
        IAccessionRepository accessionRepository,
        IUnitOfWork unitOfWork,
        PeakLimsDbContext dbContext, 
        IHeimGuardClient heimGuard)
        : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await dbContext.GetAccessionAggregate()
                .GetById(request.AccessionId, cancellationToken: cancellationToken);
            accession.MustBeFoundOrThrow();
            accession.RemoveHealthcareOrganization();

            accessionRepository.Update(accession);
            return await unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}