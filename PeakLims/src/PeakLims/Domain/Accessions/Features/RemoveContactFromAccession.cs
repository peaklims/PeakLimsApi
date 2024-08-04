namespace PeakLims.Domain.Accessions.Features;

using AccessionContacts;
using AccessionContacts.Services;
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

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IAccessionRepository _accessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionContactRepository accessionContactRepository, IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionContactRepository = accessionContactRepository;
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var orgContact = await _accessionContactRepository.GetById(request.AccessionContactId, cancellationToken: cancellationToken);
            var accession = await _accessionRepository.Query()
                .Include(x => x.AccessionContacts)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
            {
                throw new NotFoundException(nameof(Accession), request.AccessionId);
            }
            
            accession.RemoveContact(orgContact);
            _accessionContactRepository.Remove(orgContact);
            
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}