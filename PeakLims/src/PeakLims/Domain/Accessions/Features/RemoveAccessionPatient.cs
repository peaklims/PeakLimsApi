namespace PeakLims.Domain.Accessions.Features;

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

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessions);

            var accession = await _accessionRepository.Query()
                .Include(x => x.Patient)
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if(accession == null)
                throw new NotFoundException(nameof(Accession), request.AccessionId);
            
            accession.RemovePatient();

            _accessionRepository.Update(accession);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}