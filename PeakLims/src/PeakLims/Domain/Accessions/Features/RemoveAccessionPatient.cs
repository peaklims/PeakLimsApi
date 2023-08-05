namespace PeakLims.Domain.Accessions.Features;

using Patients.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class RemoveAccessionPatient
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;

        public Command(Guid accessionId)
        {
            AccessionId = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessions);

            var accession = await _accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            accession.RemovePatient();

            _accessionRepository.Update(accession);
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}