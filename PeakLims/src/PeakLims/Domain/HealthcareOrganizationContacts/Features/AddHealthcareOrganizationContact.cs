namespace PeakLims.Domain.HealthcareOrganizationContacts.Features;

using PeakLims.Domain.HealthcareOrganizationContacts.Services;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using Mappings;
using MediatR;

public static class AddHealthcareOrganizationContact
{
    public sealed class Command : IRequest<HealthcareOrganizationContactDto>
    {
        public readonly HealthcareOrganizationContactForCreationDto HealthcareOrganizationContactToAdd;

        public Command(HealthcareOrganizationContactForCreationDto healthcareOrganizationContactToAdd)
        {
            HealthcareOrganizationContactToAdd = healthcareOrganizationContactToAdd;
        }
    }

    public sealed class Handler : IRequestHandler<Command, HealthcareOrganizationContactDto>
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

        public async Task<HealthcareOrganizationContactDto> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanAddHealthcareOrganizationContacts);

            var healthcareOrganizationContactToAdd = request.HealthcareOrganizationContactToAdd.ToHealthcareOrganizationContactForCreation();
            var healthcareOrganizationContact = HealthcareOrganizationContact.Create(healthcareOrganizationContactToAdd);

            await _healthcareOrganizationContactRepository.Add(healthcareOrganizationContact, cancellationToken);
            await _unitOfWork.CommitChanges(cancellationToken);

            return healthcareOrganizationContact.ToHealthcareOrganizationContactDto();
        }
    }
}