namespace PeakLims.Domain.HealthcareOrganizationContacts.Features;

using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Services;
using PeakLims.Wrappers;
using SharedKernel.Exceptions;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;

public static class GetContactsForAnOrganization
{
    public sealed record Query(Guid OrganizationId) : IRequest<List<HealthcareOrganizationContactDto>>;

    public sealed class Handler : IRequestHandler<Query, List<HealthcareOrganizationContactDto>>
    {
        private readonly IHealthcareOrganizationContactRepository _healthcareOrganizationContactRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationContactRepository healthcareOrganizationContactRepository, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationContactRepository = healthcareOrganizationContactRepository;
            _heimGuard = heimGuard;
        }

        public async Task<List<HealthcareOrganizationContactDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadHealthcareOrganizationContacts);
            return await _healthcareOrganizationContactRepository.Query()
                .Include(x => x.HealthcareOrganization)
                .Where(x => x.HealthcareOrganization.Id == request.OrganizationId)
                .AsNoTracking()
                .ToHealthcareOrganizationContactDtoQueryable()
                ?.ToListAsync(cancellationToken)! ?? new List<HealthcareOrganizationContactDto>();
        }
    }
}