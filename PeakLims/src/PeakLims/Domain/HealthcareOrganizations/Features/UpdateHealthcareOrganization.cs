namespace PeakLims.Domain.HealthcareOrganizations.Features;

using PeakLims.Domain.HealthcareOrganizations;
using PeakLims.Domain.HealthcareOrganizations.Dtos;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Services;
using PeakLims.Domain.HealthcareOrganizations.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateHealthcareOrganization
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly HealthcareOrganizationForUpdateDto UpdatedHealthcareOrganizationData;

        public Command(Guid id, HealthcareOrganizationForUpdateDto updatedHealthcareOrganizationData)
        {
            Id = id;
            UpdatedHealthcareOrganizationData = updatedHealthcareOrganizationData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateHealthcareOrganizations);

            var healthcareOrganizationToUpdate = await _healthcareOrganizationRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var healthcareOrganizationToAdd = request.UpdatedHealthcareOrganizationData.ToHealthcareOrganizationForUpdate();
            healthcareOrganizationToUpdate.Update(healthcareOrganizationToAdd);

            _healthcareOrganizationRepository.Update(healthcareOrganizationToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}