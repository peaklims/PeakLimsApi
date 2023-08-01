namespace PeakLims.Domain.HealthcareOrganizationContacts.Features;

using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Services;
using PeakLims.Services;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class UpdateHealthcareOrganizationContact
{
    public sealed class Command : IRequest
    {
        public readonly Guid Id;
        public readonly HealthcareOrganizationContactForUpdateDto UpdatedHealthcareOrganizationContactData;

        public Command(Guid id, HealthcareOrganizationContactForUpdateDto updatedHealthcareOrganizationContactData)
        {
            Id = id;
            UpdatedHealthcareOrganizationContactData = updatedHealthcareOrganizationContactData;
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IHealthcareOrganizationContactRepository _healthcareOrganizationContactRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationContactRepository healthcareOrganizationContactRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _healthcareOrganizationContactRepository = healthcareOrganizationContactRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateHealthcareOrganizationContacts);

            var healthcareOrganizationContactToUpdate = await _healthcareOrganizationContactRepository.GetById(request.Id, cancellationToken: cancellationToken);
            var healthcareOrganizationContactToAdd = request.UpdatedHealthcareOrganizationContactData.ToHealthcareOrganizationContactForUpdate();
            healthcareOrganizationContactToUpdate.Update(healthcareOrganizationContactToAdd);

            _healthcareOrganizationContactRepository.Update(healthcareOrganizationContactToUpdate);
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}