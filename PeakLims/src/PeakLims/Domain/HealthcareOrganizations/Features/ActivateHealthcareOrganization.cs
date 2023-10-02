namespace PeakLims.Domain.HealthcareOrganizations.Features;

using Exceptions;
using HeimGuard;
using MediatR;
using PeakLims.Domain;
using PeakLims.Domain.HealthcareOrganizations.Services;
using PeakLims.Services;

public static class ActivateHealthcareOrganization
{
    public sealed record Command(Guid OrgId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly IHealthcareOrganizationRepository _panelRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHeimGuardClient _heimGuard;

        public Handler(IHealthcareOrganizationRepository panelRepository, IUnitOfWork unitOfWork, IHeimGuardClient heimGuard)
        {
            _panelRepository = panelRepository;
            _unitOfWork = unitOfWork;
            _heimGuard = heimGuard;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _heimGuard.MustHavePermission<ForbiddenAccessException>(Permissions.CanActivateHealthcareOrganizations);

            var panelToUpdate = await _panelRepository.GetById(request.OrgId, cancellationToken: cancellationToken);
            panelToUpdate.Activate();
            await _unitOfWork.CommitChanges(cancellationToken);
        }
    }
}