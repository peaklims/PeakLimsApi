namespace PeakLims.Domain.HealthcareOrganizationContacts.Features;

using Exceptions;
using HealthcareOrganizations.Services;
using PeakLims.Domain.HealthcareOrganizationContacts.Services;
using PeakLims.Domain.HealthcareOrganizationContacts;
using PeakLims.Domain.HealthcareOrganizationContacts.Dtos;
using PeakLims.Domain.HealthcareOrganizationContacts.Models;
using PeakLims.Services;
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
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationContactRepository healthcareOrganizationContactRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IHealthcareOrganizationRepository healthcareOrganizationRepository)
        {
            _healthcareOrganizationContactRepository = healthcareOrganizationContactRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
        }

        public async Task<HealthcareOrganizationContactDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var organization = await _healthcareOrganizationRepository.GetById(request.HealthcareOrganizationContactToAdd.HealthcareOrganizationId, cancellationToken: cancellationToken);

            var healthcareOrganizationContactToAdd = request.HealthcareOrganizationContactToAdd.ToHealthcareOrganizationContactForCreation();
            var healthcareOrganizationContact = HealthcareOrganizationContact.Create(healthcareOrganizationContactToAdd);
            await _healthcareOrganizationContactRepository.Add(healthcareOrganizationContact, cancellationToken);
            
            organization.AddContact(healthcareOrganizationContact);

            await _unitOfWork.CommitChanges(cancellationToken);

            return healthcareOrganizationContact.ToHealthcareOrganizationContactDto();
        }
    }
}