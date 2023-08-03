namespace PeakLims.Domain.Accessions.Features;

using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain.Accessions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddAccession
{
    public sealed record Command : IRequest<AccessionDto>;

    public sealed class Handler : IRequestHandler<Command, AccessionDto>
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

        public async Task<AccessionDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddAccessions);

            var accession = Accession.Create();

            await _accessionRepository.Add(accession, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return accession.ToAccessionDto();
        }
    }
}