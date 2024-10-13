namespace PeakLims.Domain.HealthcareOrganizations.Features;

using Exceptions;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Models;
using PeakLims.Services;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddHealthcareOrganization
{
    public sealed class Command(HealthcareOrganizationForCreationDto healthcareOrganizationToAdd)
        : IRequest<HealthcareOrganizationDto>
    {
        public readonly HealthcareOrganizationForCreationDto HealthcareOrganizationToAdd = healthcareOrganizationToAdd;
    }

    public sealed class Handler(
        IHealthcareOrganizationRepository healthcareOrganizationRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
        : IRequestHandler<Command, HealthcareOrganizationDto>
    {
        public async Task<HealthcareOrganizationDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var healthcareOrganizationToAdd = request.HealthcareOrganizationToAdd
                .ToHealthcareOrganizationForCreation(currentUserService.GetOrganizationId());
            var healthcareOrganization = HealthcareOrganization.Create(healthcareOrganizationToAdd);

            await healthcareOrganizationRepository.Add(healthcareOrganization, cancellationToken);
            await unitOfWork.CommitChanges(cancellationToken);

            return healthcareOrganization.ToHealthcareOrganizationDto();
        }
    }
}