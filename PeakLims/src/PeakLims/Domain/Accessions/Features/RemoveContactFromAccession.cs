namespace PeakLims.Domain.Accessions.Features;

using AccessionContacts;
using AccessionContacts.Services;
using Databases;
using Exceptions;
using HealthcareOrganizationContacts.Services;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;

public static class RemoveContactFromAccession
{
    public sealed record Command(Guid AccessionId, Guid AccessionContactId) : IRequest;

    public sealed class Handler(
        IAccessionContactRepository accessionContactRepository,
        IUnitOfWork unitOfWork,
        PeakLimsDbContext dbContext)
        : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var accession = await dbContext.GetAccessionAggregate()
                .GetById(request.AccessionId, cancellationToken: cancellationToken);
            accession.MustBeFoundOrThrow();
            
            var orgContact = accession.AccessionContacts.FirstOrDefault(x => x.Id == request.AccessionContactId);
            orgContact.MustBeFoundOrThrow();
            
            accession.RemoveContact(orgContact);
            accessionContactRepository.Remove(orgContact);
            
            await unitOfWork.CommitChanges(cancellationToken);
        }
    }
}