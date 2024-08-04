namespace PeakLims.Domain.Accessions.Features;

using Exceptions;
using PeakLims.Domain.Accessions.Dtos;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PeakLims.Services.External;

public static class GetEditableAccessionAggregate
{
    public sealed record Query(Guid AccessionId) : IRequest<EditableAccessionDto>;

    public sealed class Handler : IRequestHandler<Query, EditableAccessionDto>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IHeimGuardClient _heimGuard;
        private readonly IFileStorage _fileStorage;

        public Handler(IAccessionRepository accessionRepository, IHeimGuardClient heimGuard, IFileStorage fileStorage)
        {
            _accessionRepository = accessionRepository;
            _heimGuard = heimGuard;
            _fileStorage = fileStorage;
        }

        public async Task<EditableAccessionDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var accession = await _accessionRepository.Query()
                .Include(x => x.Patient)
                .Include(x => x.HealthcareOrganization)
                .Include(x => x.AccessionContacts)
                    .ThenInclude(x => x.HealthcareOrganizationContact)
                .Include(x => x.TestOrders)
                    .ThenInclude(x => x.Test)
                .Include(x => x.TestOrders)
                    .ThenInclude(x => x.Sample)
                .Include(x => x.PanelOrders)
                    .ThenInclude(x => x.Panel)
                .Include(x => x.PanelOrders)
                    .ThenInclude(x => x.TestOrders)
                    .ThenInclude(x => x.Test)
                .Include(x => x.PanelOrders)
                    .ThenInclude(x => x.TestOrders)
                    .ThenInclude(x => x.Sample)
                .Include(x => x.AccessionAttachments)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AccessionId, cancellationToken: cancellationToken);
            
            if (accession == null)
                throw new NotFoundException(nameof(Accession), request.AccessionId);

            return accession.ToEditableAccessionDto(_fileStorage);
        }
    }
}