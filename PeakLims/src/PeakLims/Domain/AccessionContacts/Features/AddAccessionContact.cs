namespace PeakLims.Domain.AccessionContacts.Features;

using PeakLims.Domain.AccessionContacts.Services;
using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddAccessionContact
{
    public sealed record Command(AccessionContactForCreationDto AccessionContactToAdd) : IRequest<AccessionContactDto>;

    public sealed class Handler : IRequestHandler<Command, AccessionContactDto>
    {
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionContactRepository accessionContactRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionContactRepository = accessionContactRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<AccessionContactDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddAccessionContacts);

            var accessionContactToAdd = request.AccessionContactToAdd.ToAccessionContactForCreation();
            var accessionContact = AccessionContact.Create(accessionContactToAdd);

            await _accessionContactRepository.Add(accessionContact, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return accessionContact.ToAccessionContactDto();
        }
    }
}