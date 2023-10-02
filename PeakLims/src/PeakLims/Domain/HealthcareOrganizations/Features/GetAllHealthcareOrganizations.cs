namespace PeakLims.Domain.HealthcareOrganizations.Features;

using Exceptions;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Wrappers;
using PeakLims.Resources;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QueryKit;
using QueryKit.Configuration;

public static class GetAllHealthcareOrganizations
{
    public sealed record Query() : IRequest<List<HealthcareOrganizationDto>>;

    public sealed class Handler : IRequestHandler<Query, List<HealthcareOrganizationDto>>
    {
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationRepository healthcareOrganizationRepository, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
            _heimGuard = heimGuard;
        }

        public async Task<List<HealthcareOrganizationDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanReadHealthcareOrganizations);
            return await _healthcareOrganizationRepository.Query()
                .AsNoTracking()
                .ToHealthcareOrganizationDtoQueryable()
                .ToListAsync(cancellationToken);
        }
    }
}