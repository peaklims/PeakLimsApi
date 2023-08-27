namespace PeakLims.Domain.AccessionContacts.Features;

using PeakLims.Domain.AccessionContacts;
using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Services;
using PeakLims.Services;
using PeakLims.Domain.AccessionContacts.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateAccessionContact
{
    public sealed record Command(Guid Id, AccessionContactForUpdateDto UpdatedAccessionContactData) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessionContacts);

            var accessionContactToUpdate = await _accessionContactRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var accessionContactToAdd = request.UpdatedAccessionContactData.ToAccessionContactForUpdate();
            accessionContactToUpdate.Update(accessionContactToAdd);

            _accessionContactRepository.Update(accessionContactToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}