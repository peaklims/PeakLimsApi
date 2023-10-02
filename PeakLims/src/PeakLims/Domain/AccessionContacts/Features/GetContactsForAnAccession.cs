namespace PeakLims.Domain.AccessionContacts.Features;

using Accessions.Services;
using Exceptions;
using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using Microsoft.EntityFrameworkCore;
using MediatR;

public static class GetContactsForAnAccession
{
    public sealed record Query(Guid AccessionId) : IRequest<List<AccessionContactDto>>;

    public sealed class Handler : IRequestHandler<Query, List<AccessionContactDto>>
    {
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionContactRepository accessionContactRepository, IHeimGuardClient heimGuard)
        {
            _accessionContactRepository = accessionContactRepository;
            _heimGuard = heimGuard;
        }

        public async Task<List<AccessionContactDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessionContacts);
            return await _accessionContactRepository.Query()
                .Where(x => x.Accession.Id == request.AccessionId)
                .AsNoTracking()
                .ToAccessionContactDtoQueryable()
                .ToListAsync(cancellationToken);
        }
    }
}