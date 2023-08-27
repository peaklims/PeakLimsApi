namespace PeakLims.Domain.Accessions.Features;

using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;

public static class GetEditableAccessionAggregate
{
    public sealed record Query(Guid AccessionId) : IRequest<EditableAccessionDto>;

    public sealed class Handler : IRequestHandler<Query, EditableAccessionDto>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IHeimGuardClient heimGuard)
        {
            _accessionRepository = accessionRepository;
            _heimGuard = heimGuard;
        }

        public async Task<EditableAccessionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessions);

            var accession = await _accessionRepository.Query()
                .Include(x => x.Patient)
                .Include(x => x.HealthcareOrganization)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
                throw new NotFoundException(nameof(Accession), request.AccessionId);

            return accession.ToEditableAccessionDto();
        }
    }
}