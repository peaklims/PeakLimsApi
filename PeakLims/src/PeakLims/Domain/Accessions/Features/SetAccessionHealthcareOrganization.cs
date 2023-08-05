namespace PeakLims.Domain.Accessions.Features;

using HealthcareOrganizations.Services;
using PeakLims.Domain.Accessions.Services;
using PeakLims.Services;
using SharedKernel.Exceptions;
using PeakLims.Domain;
using HeimGuard;
using MediatR;

public static class SetAccessionHealthcareOrganization
{
    public sealed class Command : IRequest<bool>
    {
        public readonly Guid AccessionId;
        public readonly Guid HealthcareOrganizationId;

        public Command(Guid accessionId, Guid healthcareOrganizationId)
        {
            AccessionId = accessionId;
            HealthcareOrganizationId = healthcareOrganizationId;
        }
    }

    public sealed class Handler : IRequestHandler<Command, bool>
    {
        private readonly IAccessionRepository _accessionRepository;
        private readonly IHealthcareOrganizationRepository _healthcareOrganizationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IAccessionRepository accessionRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard, IHealthcareOrganizationRepository healthcareOrganizationRepository)
        {
            _accessionRepository = accessionRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
            _healthcareOrganizationRepository = healthcareOrganizationRepository;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanUpdateAccessions);

            var accession = await _accessionRepository.GetById(request.AccessionId, cancellationToken: cancellationToken);
            var healthcareOrganization = await _healthcareOrganizationRepository.GetById(request.HealthcareOrganizationId, cancellationToken: cancellationToken);
            accession.SetHealthcareOrganization(healthcareOrganization);

            _accessionRepository.Update(accession);
            return await _unitOfWork.CommitChanges(cancellationToken) >= 1;
        }
    }
}