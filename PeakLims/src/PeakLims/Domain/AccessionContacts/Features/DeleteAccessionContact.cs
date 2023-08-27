namespace PeakLims.Domain.AccessionContacts.Features;

using PeakLims.Domain.AccessionContacts.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class DeleteAccessionContact
{
    public sealed record Command(Guid Id) : IRequest;

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
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanDeleteAccessionContacts);

            var recordToDelete = await _accessionContactRepository.GetById(request.Id, cancellationToken: cancellationToken);
            _accessionContactRepository.Remove(recordToDelete);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}