namespace PeakLims.SharedTestHelpers.Fakes.HealthcareOrganization;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Services;

public static class ActivateHealthcareOrganization
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid Id;

        public Command(Guid accessionId)
        {
            Id = accessionId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
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

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var healthcareOrganizationToUpdate = await _healthcareOrganizationRepository.GetById(request.Id, cancellationToken: cancellationToken);
            healthcareOrganizationToUpdate.Activate();
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}