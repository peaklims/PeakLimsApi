namespace PeakLims.Domain.HealthcareOrganizations.Features;

using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddHealthcareOrganization
{
    public sealed class Command : IRequest<HealthcareOrganizationDto>
    {
        public readonly HealthcareOrganizationForCreationDto HealthcareOrganizationToAdd;

        public Command(HealthcareOrganizationForCreationDto healthcareOrganizationToAdd)
        {
            HealthcareOrganizationToAdd = healthcareOrganizationToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, HealthcareOrganizationDto>
    {
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationRepository healthcareOrganizationRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task<HealthcareOrganizationDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddHealthcareOrganizations);

            var healthcareOrganizationToAdd = request.HealthcareOrganizationToAdd.ToHealthcareOrganizationForCreation();
            var healthcareOrganization = HealthcareOrganization.Create(healthcareOrganizationToAdd);

            await _healthcareOrganizationRepository.Add(healthcareOrganization, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return healthcareOrganization.ToHealthcareOrganizationDto();
        }
    }
}