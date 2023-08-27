namespace PeakLims.Domain.AccessionContacts.Features;

using PeakLims.Domain.AccessionContacts.Dtos;
using PeakLims.Domain.AccessionContacts.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class GetAccessionContact
{
    public sealed record Query(Guid Id) : IRequest<AccessionContactDto>;

    public sealed class Handler : IRequestHandler<Query, AccessionContactDto>
    {
        private readonly IAccessionContactRepository _accessionContactRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionContactRepository accessionContactRepository, IHeimGuardClient heimGuard)
        {
            _accessionContactRepository = accessionContactRepository;
            _heimGuard = heimGuard;
        }

        public async Task<AccessionContactDto> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadAccessionContacts);

            var result = await _accessionContactRepository.GetById(request.Id, cancellationToken: cancellationToken);
            return result.ToAccessionContactDto();
        }
    }
}