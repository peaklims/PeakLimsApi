namespace PeakLims.Domain.Accessions.Features;

using AccessionContacts;
using AccessionContacts.Dtos;
using AccessionContacts.Mappings;
using AccessionContacts.Services;
using Exceptions;
using HealthcareOrganizationContacts.Services;
using HeimGuard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Domain;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;

public static class AddContactToAccession
{
    public sealed record Command(Guid AccessionId, Guid HeathcareOrganizationContactId) : IRequest<AccessionContactDto>;

    public sealed class Handler : IRequestHandler<Command, AccessionContactDto>
    {
        private readonly IHealthcareOrganizationContactRepository _healthcareOrganizationContactRepository;
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IAccessionRepository _accessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationContactRepository healthcareOrganizationContactRepository, IAccessionContactRepository accessionContactRepository, IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationContactRepository = healthcareOrganizationContactRepository;
            _accessionContactRepository = accessionContactRepository;
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<AccessionContactDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var orgContact = await _healthcareOrganizationContactRepository.GetById(request.HeathcareOrganizationContactId, cancellationToken: cancellationToken);
            var accession = await _accessionRepository.Query()
                .Include(x => x.AccessionContacts)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
            {
                throw new NotFoundException(nameof(Accession), request.AccessionId);
            }

            var accessionContact = AccessionContact.Create(orgContact);
            await _accessionContactRepository.Add(accessionContact, cancellationToken);
            
            accession.AddContact(accessionContact);
            await _unitOfWork.CommitChanges(cancellationToken);

            return accessionContact.ToAccessionContactDto();
        }
    }
}